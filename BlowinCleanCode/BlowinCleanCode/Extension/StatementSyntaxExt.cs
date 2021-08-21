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
            foreach (var node in self.DescendantNodesAndSelf(e => !e.Is<BlockSyntax>()))
            {
                if(!node.Is<StatementSyntax>(out var statementSyntax))
                    continue;
                
                count += 
                    // {}
                    statementSyntax is BlockSyntax bs ? 
                        bs.DescendantNodes().CountOfLines() : 
                        1;
            }

            return count;
        }
    }
}