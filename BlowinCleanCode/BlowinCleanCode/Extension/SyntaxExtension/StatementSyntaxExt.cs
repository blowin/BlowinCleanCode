using System;
using Microsoft.CodeAnalysis;

namespace BlowinCleanCode.Extension.SyntaxExtension
{
    public static class StatementSyntaxExt
    {
        public static int CountOfLines<T>(this SyntaxList<T> self)
            where T : SyntaxNode
        {
            var bodyStr = self.ToString();
            var count = 0;
            foreach (var stringSlice in bodyStr.SplitEnumerator(Environment.NewLine))
            {
                if(stringSlice.Equals(string.Empty))
                    continue;

                count++;
            }

            return count;
        }
    }
}