using System;
using System.Collections.Generic;
using System.Linq;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.CodeSmell
{
    public sealed class SwitchStatementsShouldNotBeNestedFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase<MethodDeclarationSyntax>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.SwitchStatementsShouldNotBeNested, 
            title: "\"switch\" statements should not be nested",
            messageFormat: "\"switch\" statements should not be nested", 
            Constant.Category.CodeSmell,
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true);
        
        protected override SyntaxKind SyntaxKind => SyntaxKind.MethodDeclaration;
        
        protected override void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax syntaxNode)
        {
            foreach (var rootSwitchStatement in ChildSwitchStatement(syntaxNode, node => !node.Is<SwitchStatementSyntax>()))
            {
                if(AnalyzerCommentSkipCheck.Skip(rootSwitchStatement))
                    continue;

                foreach (var switchStatementSyntax in ChildSwitchStatement(rootSwitchStatement, null))
                    ReportDiagnostic(context, switchStatementSyntax.GetLocation());
            }
        }

        private static IEnumerable<SwitchStatementSyntax> ChildSwitchStatement(SyntaxNode node, Func<SyntaxNode, bool> checkChild)
        {
            return checkChild == null ? 
                node.DescendantNodes().OfType<SwitchStatementSyntax>() : 
                node.DescendantNodes(checkChild).OfType<SwitchStatementSyntax>();
        }
    }
}