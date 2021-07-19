using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlowinCleanCode.Comment
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
            if (node is StatementSyntax)
                return true;

            if (node is MemberAccessExpressionSyntax)
                return true;
            
            return false;
        }
    }
}