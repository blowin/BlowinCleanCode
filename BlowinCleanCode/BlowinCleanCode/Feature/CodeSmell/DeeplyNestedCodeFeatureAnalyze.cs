using System.Collections.Generic;
using System.Linq;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace BlowinCleanCode.Feature.CodeSmell
{
    public sealed class DeeplyNestedCodeFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase<MethodDeclarationSyntax>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(
            Constant.Id.DeeplyNestedCode,
            title: "Deeply nested code",
            messageFormat: "Statements should not be nested too deeply",
            Constant.Category.CodeSmell,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        protected override SyntaxKind SyntaxKind => SyntaxKind.MethodDeclaration;

        protected override void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax syntaxNode)
        {
            if (syntaxNode.Body == null)
                return;

            foreach (var childNode in syntaxNode.Body.Statements)
            {
                if (AnalyzerCommentSkipCheck.Skip(childNode))
                    continue;

                foreach (var descendantNode in AllCheckStatements(childNode))
                {
                    Check(context, descendantNode);
                    if (descendantNode is IfStatementSyntax ifS && ifS.Else?.Statement != null)
                        Check(context, ifS.Else.Statement);
                }
            }
        }

        private static bool IsCheckNode(SyntaxNode node) => node.IsKind(SyntaxKind.Block);

        private void Check(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            if (AnalyzerCommentSkipCheck.Skip(node))
                return;

            var count = Depth(node);
            if (count <= Settings.MaxDeeplyNested)
                return;

            foreach (var syntaxNode in node.DescendantNodesAndSelf())
            {
                if (!(syntaxNode is BlockSyntax blockSyntax))
                    continue;

                var textSpan = TextSpan.FromBounds(blockSyntax.Parent.SpanStart, blockSyntax.OpenBraceToken.Span.End);
                var location = Location.Create(blockSyntax.SyntaxTree, textSpan);
                ReportDiagnostic(context, location);
                return;
            }
        }

        private int Depth(SyntaxNode node)
        {
            var max = 0;

            foreach (var syntaxNode in node.ChildNodes())
            {
                foreach (var allCheckStatement in AllCheckStatements(syntaxNode))
                {
                    var newDepth = Depth(allCheckStatement);
                    if (newDepth > max)
                        max = newDepth;
                }
            }

            return max + 1;
        }

        private IEnumerable<SyntaxNode> AllCheckStatements(SyntaxNode node)
        {
            return node
                .DescendantNodes(e => !IsCheckNode(e))
                .Where(e => IsCheckNode(e) && !AnalyzerCommentSkipCheck.Skip(e));
        }
    }
}
