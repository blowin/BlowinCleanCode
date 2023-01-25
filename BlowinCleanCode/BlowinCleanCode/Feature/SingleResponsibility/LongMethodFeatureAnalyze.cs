using BlowinCleanCode.Extension.SyntaxExtension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.SingleResponsibility
{
    public sealed class LongMethodFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase<MethodDeclarationSyntax>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(
            Constant.Id.LongMethod,
            title: "Method is long",
            messageFormat: "Method '{0}' too long",
            Constant.Category.SingleResponsibility,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Method must be shorter");

        protected override SyntaxKind SyntaxKind => SyntaxKind.MethodDeclaration;

        protected override void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax syntaxNode)
        {
            if (syntaxNode.Body == null || syntaxNode.Body.Statements.Count == 0)
                return;

            var lineOfCode = 0;
            foreach (var statementSyntax in syntaxNode.Body.Statements)
                lineOfCode += statementSyntax.CountOfLines();

            if (lineOfCode <= Settings.MaxCountOfLinesInMethod)
                return;

            ReportDiagnostic(context, syntaxNode.Identifier.GetLocation(), syntaxNode.Identifier.ToString());
        }
    }
}
