using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlowinCleanCode.Extension.SyntaxExtension
{
    public static class SyntaxNodeExt
    {
#pragma warning disable SA1311
#pragma warning disable IDE1006
        private static readonly string[] NewLineSeparators = new[] { "\r", "\n", Environment.NewLine };
#pragma warning restore IDE1006
#pragma warning restore SA1311

        public static int CountOfLines(this SyntaxNode self)
        {
            var bodyStr = TriviaRemover.Instance.Visit(self).ToString();
            var count = 0;
            foreach (var stringSlice in bodyStr.SplitEnumerator(NewLineSeparators))
            {
                if (stringSlice.Trim().IsEmpty)
                    continue;

                count++;
            }

            return count;
        }

        public static SyntaxNode RemoveParentheses(this SyntaxNode self)
        {
            var current = self;
            while (current is ParenthesizedExpressionSyntax parenthesized)
                current = parenthesized.Expression;
            return current;
        }

        public static IEnumerable<BinaryExpressionSyntax> FlatBinaryExpression(this SyntaxNode node, HashSet<SyntaxNode> visitedSet)
        {
            while (true)
            {
                var normalizeNode = node?.RemoveParentheses();
                if (!(normalizeNode is BinaryExpressionSyntax binaryNode) || !binaryNode.Kind().In(SyntaxKind.LogicalOrExpression, SyntaxKind.LogicalAndExpression) || !visitedSet.Add(binaryNode))
                    yield break;

                foreach (var flatBinaryExpressionSyntax in FlatBinaryExpression(binaryNode.Left, visitedSet))
                    yield return flatBinaryExpressionSyntax;

                yield return binaryNode;

                node = binaryNode.Right;
            }
        }

        private sealed class TriviaRemover : CSharpSyntaxRewriter
        {
            public static CSharpSyntaxRewriter Instance = new TriviaRemover();

            public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
            {
                var kind = trivia.Kind();
                if (kind.In(
                        SyntaxKind.MultiLineCommentTrivia,
                        SyntaxKind.SingleLineCommentTrivia,
                        SyntaxKind.XmlComment,
                        SyntaxKind.XmlCommentEndToken,
                        SyntaxKind.XmlCommentStartToken,
                        SyntaxKind.DocumentationCommentExteriorTrivia))
                {
                    return default(SyntaxTrivia);
                }

                return base.VisitTrivia(trivia);
            }
        }
    }
}
