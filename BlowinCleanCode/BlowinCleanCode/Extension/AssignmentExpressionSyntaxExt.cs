using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlowinCleanCode.Extension
{
    public static class AssignmentExpressionSyntaxExt
    {
        public static bool IsCreationAssignment(this AssignmentExpressionSyntax self)
        {
            return self.IsKind(SyntaxKind.SimpleAssignmentExpression) && self.Right.IsCreation();
        }
    }
}