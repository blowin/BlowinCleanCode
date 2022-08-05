using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlowinCleanCode.Extension.SyntaxExtension
{
    public static class SyntaxNodeExt
    {
        public static SyntaxNode RemoveParentheses(this SyntaxNode self)
        {
            var current = self;
            while (current is ParenthesizedExpressionSyntax parenthesized)
                current = parenthesized.Expression;
            return current;
        }
    }
}