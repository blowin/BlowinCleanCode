using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlowinCleanCode.Model.Comment
{
    public ref struct PlaceForCommentFinder
    {
        public SyntaxNode Find(SyntaxNode node)
        {
            if (IsFindNode(node))
                return node;

            foreach (var syntaxNode in node.Ancestors())
            {
                if (IsFindNode(syntaxNode))
                    return syntaxNode;
            }

            return node;
        }

        private static bool IsFindNode(SyntaxNode node)
        {
            switch (node)
            {
                case StatementSyntax _:
                case MemberAccessExpressionSyntax _:
                case TypeDeclarationSyntax _:
                case MethodDeclarationSyntax _:
                    return true;
                default:
                    return false;
            }
        }
    }
}
