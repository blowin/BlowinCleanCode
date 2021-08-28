using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlowinCleanCode.Extension
{
    public static class MethodDeclarationSyntaxExt
    {
        public static bool HasBodyOrExpressionBody(this BaseMethodDeclarationSyntax self) =>
            self.Body != null || self.ExpressionBody != null;

        public static IEnumerable<SyntaxNode> GetBodyDescendantNodes(this BaseMethodDeclarationSyntax self)
        {
            if (self.Body != null)
                return self.Body.DescendantNodes();

            if (self.ExpressionBody != null)
                return self.ExpressionBody.DescendantNodes();

            throw new ArgumentNullException(nameof(self));
        }

        public static bool IsExtension(this MethodDeclarationSyntax self)
        {
            var parameters = self.ParameterList.Parameters;
            if (parameters.Count == 0)
                return false;

            return parameters[0].Modifiers.Any(SyntaxKind.ThisKeyword);
        }
    }
}