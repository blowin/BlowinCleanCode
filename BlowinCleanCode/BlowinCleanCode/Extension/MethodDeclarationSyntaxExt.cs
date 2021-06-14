using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlowinCleanCode.Extension
{
    public static class MethodDeclarationSyntaxExt
    {
        public static bool IsExtension(this MethodDeclarationSyntax self)
        {
            var parameters = self.ParameterList.Parameters;
            if (parameters.Count == 0)
                return false;

            return parameters[0].Modifiers.Any(SyntaxKind.ThisKeyword);
        }
    }
}