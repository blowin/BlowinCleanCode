using BlowinCleanCode.Extension.SyntaxExtension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.SingleResponsibility
{
    public sealed class SwitchShouldNotHaveALotOfCasesFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase<SwitchStatementSyntax>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(
            Constant.Id.SwitchShouldNotHaveALotOfCases,
            title: "\"switch\" have a lot of \"case\" clauses",
            messageFormat: "\"switch\" have a lot of \"case\" clauses {0}/{1}",
            Constant.Category.CodeSmell,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Relatively rare use of switch and case operators is one of the hallmarks of object-oriented code. Often code for a single switch can be scattered in different places in the program. When a new condition is added, you have to find all the switch code and modify it. As a rule of thumb, when you see switch you should think of polymorphism.");

        protected override SyntaxKind SyntaxKind => SyntaxKind.SwitchStatement;

        protected override void Analyze(SyntaxNodeAnalysisContext context, SwitchStatementSyntax syntaxNode)
        {
            var countOfCases = syntaxNode.CountOfCases();
            if (countOfCases <= Settings.MaxSwitchCaseCount)
                return;

            ReportDiagnostic(context, syntaxNode.SwitchKeyword.GetLocation(), countOfCases, Settings.MaxSwitchCaseCount);
        }
    }
}
