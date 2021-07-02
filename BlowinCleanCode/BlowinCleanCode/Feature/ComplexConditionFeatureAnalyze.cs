using System.Linq;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature
{
    public sealed class ComplexConditionFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase<MethodDeclarationSyntax>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.ComplexCondition, 
            title: "Condition expression too complex",
            messageFormat: "The expression in the condition is too complex", 
            Constant.Category.SingleResponsibility, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true);
        
        protected override SyntaxKind SyntaxKind => SyntaxKind.MethodDeclaration;
        
        protected override void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax syntaxNode)
        {
            foreach (var descendantNode in syntaxNode.DescendantNodes())
            {
                switch (descendantNode)
                {
                    case IfStatementSyntax _:
                    case VariableDeclarationSyntax _:
                    case WhileStatementSyntax _:
                    case ReturnStatementSyntax _:
                    case ArgumentSyntax _:
                        var (node, countOfCondition) = CountOfCondition(descendantNode);
                        if(countOfCondition > Settings.MaxCountOfCondition)
                            ReportDiagnostic(context, node.GetLocation());
                        break;
                }
            }
        }

        private static bool IsLogicalBinaryExpression(BinaryExpressionSyntax bes)
        {
            switch (bes.Kind())
            {
                case SyntaxKind.LogicalAndExpression:
                case SyntaxKind.LogicalOrExpression:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsNodeForCheckChild(SyntaxNode node)
        {
            switch (node)
            {
                case BinaryExpressionSyntax bes:
                    return IsLogicalBinaryExpression(bes);
                case ParenthesizedExpressionSyntax _:
                    return true;
                default:
                    return false;
            }
        }
        
        private static (SyntaxNode, int) CountOfCondition(SyntaxNode node)
        {
            var firstBinaryExpression = node.DescendantNodes().OfType<BinaryExpressionSyntax>().FirstOrDefault();
            if (firstBinaryExpression == null)
                return (node, 0);

            if (!IsLogicalBinaryExpression(firstBinaryExpression))
                return (firstBinaryExpression, 1);

            return (firstBinaryExpression, firstBinaryExpression
                .DescendantNodes(e => IsNodeForCheckChild(e))
                .Count(e => !IsNodeForCheckChild(e)));
        }
    }
}