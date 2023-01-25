using System.Collections.Generic;
using System.Linq;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.CodeSmell.MagicValue
{
    public sealed class MagicValueFeatureSymbolAnalyze : FeatureSyntaxNodeAnalyzerBase<MethodDeclarationSyntax>
    {
        private static readonly MagicValueSkipCheckDescendantNodesVisitor MagicValueSkipCheckDescendantNodesVisitor =
            new MagicValueSkipCheckDescendantNodesVisitor();

        private static readonly List<string> SkipLiteralValues = new List<string>
        {
            "0",
            "1",
            "-1",
        };

        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(
            Constant.Id.MagicValue,
            title: "Expression shouldn't contain magic value",
            messageFormat: "Magic value '{0}'",
            Constant.Category.CodeSmell,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        protected override SyntaxKind SyntaxKind => SyntaxKind.MethodDeclaration;

        protected override void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax syntax)
        {
            if (MagicValueSkipCheckDescendantNodesVisitor.Visit(syntax))
                return;

            foreach (var literal in Literals(syntax, context))
            {
                if (AnalyzerCommentSkipCheck.Skip(literal))
                    continue;

                if (SkipLiteral(literal))
                    continue;

                ReportDiagnostic(context, literal.GetLocation(), literal.ToFullString());
            }
        }

        private static bool SkipLiteral(LiteralExpressionSyntax node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.StringLiteralExpression:
                    return node.Parent is BinaryExpressionSyntax && !node.Parent.Kind().IsLogicalOperator();
                case SyntaxKind.NullLiteralExpression:
                    return true;
                default:
                    if (SkipLiteralValues.Contains(node.Token.ValueText ?? string.Empty))
                        return true;

                    return false;
            }
        }

        private IEnumerable<LiteralExpressionSyntax> Literals(MethodDeclarationSyntax syntax, SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            var literalExtractorVisitor = new MagicValueLiteralExtractorVisitor(MagicValueSkipCheckDescendantNodesVisitor, syntaxNodeContext);
            foreach (var node in syntax.DescendantNodes(n => !MagicValueSkipCheckDescendantNodesVisitor.Visit(n)))
            {
                if (!(node is CSharpSyntaxNode cSharpSyntaxNode))
                    continue;

                var literals = cSharpSyntaxNode.Accept(literalExtractorVisitor) ?? Enumerable.Empty<LiteralExpressionSyntax>();
                foreach (var literalExpressionSyntax in literals)
                    yield return literalExpressionSyntax;
            }
        }
    }
}
