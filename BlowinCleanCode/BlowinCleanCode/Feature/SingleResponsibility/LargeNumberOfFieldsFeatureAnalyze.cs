using System.Linq;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Extension.SymbolExtension;
using BlowinCleanCode.Extension.SyntaxExtension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.SingleResponsibility
{
    public sealed class LargeNumberOfFieldsFeatureAnalyze : TypeDeclarationSyntaxNodeAnalyzerBase
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(
            Constant.Id.LargeNumberOfFields,
            title: "Large number of fields in type",
            messageFormat: "'{0}' has a lot of fields.",
            Constant.Category.SingleResponsibility,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        protected override void Analyze(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax syntaxNode)
        {
            if (!context.ContainingSymbol.Is<INamedTypeSymbol>(out var namedTypeSymbol))
                return;

            if (namedTypeSymbol.HasUserMethods() && CountOfFields(namedTypeSymbol) > Settings.MaxNumberOfField)
                ReportDiagnostic(context, syntaxNode.Identifier.GetLocation(), syntaxNode.TypeName());
        }

        private static int CountOfFields(INamedTypeSymbol type) => type.Fields().Count(e => !e.IsConst);
    }
}
