using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlowinCleanCode.Extension
{
    public static class TypeSyntaxExt
    {
        public static bool IsBool(this TypeSyntax self)
        {
            if (!(self is PredefinedTypeSyntax pts))
                return false;
            
            return pts.IsBool();
        }
    }
}