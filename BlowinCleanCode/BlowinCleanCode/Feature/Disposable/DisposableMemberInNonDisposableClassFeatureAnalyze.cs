using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.Disposable
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
            
            var invalidFields = AllDisposableFields(symbol).Select(s => s.NormalizeName());
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
        
        private static IEnumerable<ISymbol> AllDisposableFields(INamedTypeSymbol symbol)
        {
            return symbol.GetMembers().OfType<IFieldSymbol>()
                .Where(f => ImplementDisposable(f.Type))
                .Select(f => f);
        }
        
        private static bool ImplementDisposable(ITypeSymbol symbol) 
            => symbol.AllInterfaces.Any(implInterface => implInterface.IsDisposableOrAsyncDisposable());
    }
}