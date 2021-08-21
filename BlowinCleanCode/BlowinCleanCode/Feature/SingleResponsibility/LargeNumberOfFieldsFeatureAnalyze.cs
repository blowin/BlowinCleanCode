using System.Linq;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.SingleResponsibility
{
    public sealed class LargeNumberOfFieldsFeatureAnalyze : TypeDeclarationSyntaxNodeAnalyzerBase
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.LargeNumberOfFields, 
            title: "Large number of fields in type",
            messageFormat: "'{0}' has a lot of fields.", 
            Constant.Category.SingleResponsibility,
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true);
  
        protected override void Analyze(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax syntaxNode)
        {
            if(!context.ContainingSymbol.Is<INamedTypeSymbol>(out var namedTypeSymbol))
                return;
            
            var identifier = syntaxNode.Identifier;
            var name = identifier.Text ?? string.Empty;

            if(HasUserMethods(namedTypeSymbol) && CountOfFields(namedTypeSymbol) > Settings.MaxNumberOfField)
                ReportDiagnostic(context, identifier.GetLocation(), name);
        }

        private static bool HasUserMethods(INamedTypeSymbol type)
        {
            foreach (var member in type.GetMembers())
            {
                if(!member.Is<IMethodSymbol>(out var methodSymbol))
                    continue;

                if (methodSymbol.MethodKind == MethodKind.Ordinary)
                    return !methodSymbol.Name.In(nameof(Equals), nameof(GetHashCode), nameof(ToString));
            }

            return false;
        }

        private static int CountOfFields(INamedTypeSymbol type) => type.GetMembers().OfType<IFieldSymbol>().Count();
    }
}