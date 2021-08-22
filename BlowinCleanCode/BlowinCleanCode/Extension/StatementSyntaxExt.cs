using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlowinCleanCode.Extension
{
    public static class StatementSyntaxExt
    {
        public static int CountOfLines(this IEnumerable<SyntaxNode> self)
        {
            var count = 0;
            foreach (var syntaxNode in self)
                count += syntaxNode.CountOfLines();
            return count;
        }
        
        public static int CountOfLines(this SyntaxNode self)
        {
            var count = 0;
            foreach (var node in self.DescendantNodesAndSelf())
            {
                if(!node.Is<StatementSyntax>())
                    continue;
                
                count += 1;
            }

            return count;
        }
    }
}