using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlowinCleanCode.Extension
{
    public static class ExpressionSyntaxExt
    {
        private static readonly string[] CreationMethodPatternNames = new[]
        {
            "Open",
            "From",
            "Build",
            "Create",
        };
        
        public static bool IsCreation(this ExpressionSyntax self)
        {
            if (self is InvocationExpressionSyntax invocation &&
                invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                var methodName = memberAccess.Name.ToString();
                return methodName.StartWithAny(CreationMethodPatternNames);
            }
            
            return self is ObjectCreationExpressionSyntax;
        }
    }
}