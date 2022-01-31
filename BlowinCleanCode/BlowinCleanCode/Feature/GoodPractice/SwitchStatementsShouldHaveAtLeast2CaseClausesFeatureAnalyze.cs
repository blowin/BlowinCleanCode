using System;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Extension.SyntaxExtension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.GoodPractice
{
    public class SwitchStatementsShouldHaveAtLeast2CaseClausesFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase<SwitchStatementSyntax>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.SwitchStatementsShouldHaveAtLeast2CaseClauses, 
            title: "\"switch\" statements should have at least 2 \"case\" clauses",
            messageFormat: "\"switch\" statements should have at least 2 \"case\" clauses", 
            Constant.Category.GoodPractice, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true,
            description: $"switch statements and expressions are useful when there are many different cases depending on the value of the same expression. {Environment.NewLine}When a switch statement or expression is simple enough, the code will be more readable with a single if, if-else or ternary conditional operator.");
        
        protected override SyntaxKind SyntaxKind => SyntaxKind.SwitchStatement;
        
        protected override void Analyze(SyntaxNodeAnalysisContext context, SwitchStatementSyntax syntaxNode)
        {
            if(syntaxNode.CountOfCases() >= 2)
                return;
            
            ReportDiagnostic(context, syntaxNode.GetLocation());
        }
    }
}