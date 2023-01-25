using BlowinCleanCode.Feature.Base;
using BlowinCleanCode.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.SingleResponsibility
{
    public sealed class CognitiveComplexityMethodFeatureAnalyze : FeatureSymbolAnalyzeBase<IMethodSymbol>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(
            Constant.Id.CognitiveComplexity,
            title: "The method has a coherent cognitive complexity.",
            messageFormat: "The method '{0}' has a coherent cognitive complexity. {1}",
            Constant.Category.SingleResponsibility,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "The method should be simpler.");

        protected override SymbolKind SymbolKind => SymbolKind.Method;

        protected override void Analyze(SymbolAnalysisContext context, IMethodSymbol ms)
        {
            if (!ms.IsDefinition)
                return;

            var walker = new ComplexityWalker(context.Compilation);
            foreach (var reference in ms.DeclaringSyntaxReferences)
            {
                var syntax = reference.GetSyntax(context.CancellationToken) as MethodDeclarationSyntax;
                if (syntax == null)
                    continue;

                var complexitySettings = Settings.CognitiveComplexity;
                var actualComplexity = walker.Complexity(syntax);
                if (complexitySettings.TryBuildMessage(actualComplexity, out var additionalInformation))
                    ReportDiagnostic(context, syntax.Identifier.GetLocation(), syntax.Identifier.ToString(), additionalInformation);
            }
        }
    }
}
