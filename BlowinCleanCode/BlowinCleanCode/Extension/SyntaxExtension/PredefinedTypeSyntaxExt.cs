using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlowinCleanCode.Extension.SyntaxExtension
{
    public static class PredefinedTypeSyntaxExt
    {
        public static bool IsBool(this PredefinedTypeSyntax self)
        {
            var returnKeyword = self.Keyword.Kind();
            return returnKeyword == SyntaxKind.BoolKeyword;
        }
    }
}