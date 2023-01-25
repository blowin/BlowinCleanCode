using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.GoodPractice
{
    public sealed class NameTooLongFeatureAnalyze : IdentifierNameSyntaxAnalyzerBase
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(
            Constant.Id.NameTooLong,
            title: "The name is too long.",
            messageFormat: "The name \"{0}\" is too long.",
            Constant.Category.GoodPractice,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        protected override void Analyze(SyntaxNodeAnalysisContext context, SyntaxToken syntaxNode)
        {
            var name = syntaxNode.Text ?? string.Empty;
            if (name.Length > Settings.MaxNameLength)
                ReportDiagnostic(context, syntaxNode.GetLocation(), name);
        }
    }
}
