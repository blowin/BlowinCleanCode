using System.Linq;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Extension.SyntaxExtension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.CodeSmell
{
    public sealed class MethodShouldNotHaveManyReturnStatementsFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase<MethodDeclarationSyntax>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(
            Constant.Id.MethodShouldNotHaveManyReturnStatements,
            title: "Methods should not have too many return statements",
            messageFormat: "Methods should not have too many return statements {0}/{1}",
            Constant.Category.CodeSmell,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Having too many return statements in a method increases the method’s essential complexity because the flow of execution is broken each time a return statement is encountered. This makes it harder to read and understand the logic of the method.");

        protected override SyntaxKind SyntaxKind => SyntaxKind.MethodDeclaration;

        protected override void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax syntaxNode)
        {
            var maxReturnStatement = syntaxNode.ReturnType.IsBool()
                ? Settings.MaxReturnStatementForReturnBool
                : Settings.MaxReturnStatement;

            var count = CountOfReturnStatements(syntaxNode);
            if (count <= maxReturnStatement)
                return;

            ReportDiagnostic(context, syntaxNode.Identifier.GetLocation(), count, maxReturnStatement);
        }

        private static int CountOfReturnStatements(MethodDeclarationSyntax syntaxNode)
        {
            var count = 0;
            foreach (var descendantNode in syntaxNode.DescendantNodes(node => node.IsNot<LambdaExpressionSyntax>() && node.IsNot<SwitchStatementSyntax>()))
                count += CountOfReturns(descendantNode);

            return count;
        }

        private static int CountOfReturns(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ReturnStatementSyntax)
                return 1;

            if (syntaxNode.IsNot<SwitchStatementSyntax>())
                return 0;

            foreach (var descendantNode in syntaxNode.DescendantNodes(v => v.IsNot<LambdaExpressionSyntax>()))
            {
                if (descendantNode.IsNot<ReturnStatementSyntax>())
                    continue;

                return 1;
            }

            return 0;
        }
    }
}
