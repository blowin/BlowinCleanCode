using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace BlowinCleanCode
{
    public static class SyntaxNodeExt
    {
        public static IEnumerable<SyntaxNode> ParentNodes(this SyntaxNode self)
        {
            if(self.Parent == null)
                yield break;

            yield return self.Parent;

            foreach (var parentNode in self.Parent.ParentNodes())
                yield return parentNode;
        }
    }
}