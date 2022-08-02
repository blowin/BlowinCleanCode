using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.GoodPractice
{
    public sealed class VariableNameTooLongFeatureAnalyze : IdentifierNameSyntaxAnalyzerBase
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.VariableNameTooLong,
            title: "The name is too long.",
            messageFormat: "The name \"{0}\" is too long.",
            Constant.Category.GoodPractice,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);
        
        protected override void Analyze(SyntaxNodeAnalysisContext context, SyntaxToken syntaxNode)
        {
            var variableName = syntaxNode.Text ?? string.Empty;
            if (variableName.Length > Settings.MaxLengthVariableName)
                ReportDiagnostic(context, syntaxNode.GetLocation(), variableName);
        }
    }
}