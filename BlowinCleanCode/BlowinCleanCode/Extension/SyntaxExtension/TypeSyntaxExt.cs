using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlowinCleanCode.Extension.SyntaxExtension
{
    public static class TypeSyntaxExt
    {
        public static bool IsBool(this TypeSyntax self)
        {
            if (self is PredefinedTypeSyntax pts)
                return pts.IsBool();

            if (self is NullableTypeSyntax nts)
                return nts.ElementType.IsBool();

            return false;
        }
    }
}