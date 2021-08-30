using BlowinCleanCode.Model.Matchers;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlowinCleanCode.Extension
{
    public static class ExpressionSyntaxExt
    {
        private static readonly (string, IMatcher<string>)[] CreationMethodPatternNames = 
        {
            ("Open", StartWithMatcher.InstanceInvariantCultureIgnoreCase),
            ("From", StartWithMatcher.InstanceInvariantCultureIgnoreCase),
            ("Build", StartWithMatcher.InstanceInvariantCultureIgnoreCase),
            ("Create", StartWithMatcher.InstanceInvariantCultureIgnoreCase),
            ("Of", StringEqualityMatcher.InstanceInvariantCultureIgnoreCase),
        };
        
        public static bool IsCreation(this ExpressionSyntax self)
        {
            if (self is InvocationExpressionSyntax invocation &&
                invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                var methodName = memberAccess.Name.ToString();
                return methodName.MatchAny(CreationMethodPatternNames);
            }
            
            return self is ObjectCreationExpressionSyntax;
        }
    }
}