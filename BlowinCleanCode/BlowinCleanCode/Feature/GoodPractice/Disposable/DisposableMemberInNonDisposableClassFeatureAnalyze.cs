using System.Collections.Generic;
using System.Linq;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Extension.SymbolExtension;
using BlowinCleanCode.Extension.SyntaxExtension;
using BlowinCleanCode.Feature.Base;
using BlowinCleanCode.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.GoodPractice.Disposable
{
    public sealed class DisposableMemberInNonDisposableClassFeatureAnalyze : FeatureSymbolAnalyzeBase<INamedTypeSymbol>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.DisposableMemberInNonDisposable, 
            title: "Disposable member in non disposable",
            messageFormat: "Implement 'IDisposable' in this class and use the 'Dispose' method to call 'Dispose' on {0}.", 
            Constant.Category.GoodPractice, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true);
        
        protected override SymbolKind SymbolKind => SymbolKind.NamedType;
        
        protected override void Analyze(SymbolAnalysisContext context, INamedTypeSymbol symbol)
        {
            if(ImplementDisposable(symbol))
                return;

            var typeDeclaration = GetTypeDeclarationSyntax(symbol);
            if(typeDeclaration == null)
                return;

            var disposableFields = symbol.FieldOrProperties().Where(t => ImplementDisposable(t.Type)).ToHashSet();
            if(disposableFields.Count == 0)
                return;

            var allInitializationFromMethods = AllInitializationFromMethods(symbol, disposableFields, context.Compilation).ToHashSet();
            
            var invalidFields = disposableFields
                .Where(f => IsInPlaceInitialization(f) || allInitializationFromMethods.Contains(f))
                .Select(s => s.Name.ToString());
            
            var fields = string.Join(" and ", invalidFields);
            if(string.IsNullOrEmpty(fields))
                return;

            ReportDiagnostic(context, typeDeclaration.Identifier.GetLocation(), fields);
        }
        
        private static TypeDeclarationSyntax GetTypeDeclarationSyntax(INamedTypeSymbol symbol)
        {
            foreach (var symbolDeclaringSyntaxReference in symbol.DeclaringSyntaxReferences)
            {
                if (symbolDeclaringSyntaxReference.GetSyntax() is TypeDeclarationSyntax tds)
                    return tds;
            }

            return null;
        }
        
        private static IEnumerable<FieldOrProperty> AllInitializationFromMethods(INamedTypeSymbol symbol, HashSet<FieldOrProperty> checkFields, Compilation compilation)
        {
            return symbol.GetMembers().OfType<IMethodSymbol>()
                .SelectMany(m => AllAssignmentsFromMethod(m, compilation))
                .Where(f => checkFields.Contains(f));
        }
        
        private static IEnumerable<FieldOrProperty> AllAssignmentsFromMethod(IMethodSymbol m, Compilation compilation)
        {
            if (m.DeclaringSyntaxReferences.Length != 1)
                return Enumerable.Empty<FieldOrProperty>();
            
            if(!m.DeclaringSyntaxReferences[0].GetSyntax().Is<BaseMethodDeclarationSyntax>(out var method))
                return Enumerable.Empty<FieldOrProperty>();
            
            if (!method.HasBodyOrExpressionBody())
                return Enumerable.Empty<FieldOrProperty>();

            var semanticModel = compilation.GetSemanticModel(method.SyntaxTree);
            
            return method.GetBodyDescendantNodes()
                .OfType<AssignmentExpressionSyntax>()
                .Where(n => n.IsCreationAssignment())
                .Select(n => semanticModel.GetSymbolInfo(n.Left).Symbol)
                .Where(s => !IsBackingField(s) && FieldOrProperty.IsFieldOrProperty(s))
                .Select(s => FieldOrProperty.Create(s));
        }

        private static bool IsBackingField(ISymbol s) => s is IFieldSymbol f && f.IsBackingField();

        private static bool IsInPlaceInitialization(FieldOrProperty symbol)
        {
            var declaringSyntax = symbol.IsField
                ? symbol.Field.DeclaringSyntaxReferences
                : symbol.Property.DeclaringSyntaxReferences;
            
            var syntax = declaringSyntax.SingleOrDefault()?.GetSyntax();
            if(!syntax.Is<VariableDeclaratorSyntax>(out var declarator))
                return false;

            var initializerValue = declarator.Initializer?.Value;
            if (initializerValue == null)
                return false;
            
            return initializerValue.IsCreation();
        }
        
        private static bool ImplementDisposable(ITypeSymbol symbol) 
            => symbol.AllInterfaces.Any(implInterface => implInterface.IsDisposableOrAsyncDisposable());
    }
}