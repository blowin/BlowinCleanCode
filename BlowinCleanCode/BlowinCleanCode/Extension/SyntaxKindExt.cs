using Microsoft.CodeAnalysis.CSharp;

namespace BlowinCleanCode.Extension
{
    public static class SyntaxKindExt
    {
        public static bool IsLogicalOperator(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                    return true;
                default:
                    return false;
            }
        }
    }
}