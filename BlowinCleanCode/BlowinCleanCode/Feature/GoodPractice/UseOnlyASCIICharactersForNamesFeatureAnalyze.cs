using BlowinCleanCode.Extension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.GoodPractice
{
    public sealed class UseOnlyASCIICharactersForNamesFeatureAnalyze : IdentifierNameSyntaxAnalyzerBase
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.UseOnlyASCIICharactersForNames,
            title: "Use only ascii symbols in identifiers.",
            messageFormat: "The name \"{0}\" contains non-ascii characters.",
            Constant.Category.GoodPractice,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);
        
        protected override void Analyze(SyntaxNodeAnalysisContext context, SyntaxToken syntaxNode)
        {
            var variableName = syntaxNode.Text ?? string.Empty;
            if (!variableName.IsAscii())
                ReportDiagnostic(context, syntaxNode.GetLocation(), variableName);
        }
    }
}