using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlowinCleanCode.Extension.SyntaxExtension
{
    public static class TypeDeclarationSyntaxExt
    {
        public static string TypeName(this TypeDeclarationSyntax self)
        {
            var identifier = self.Identifier;
            return identifier.Text ?? string.Empty;
        }
    }
}
