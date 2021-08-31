using BlowinCleanCode.Model.Matchers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Primitives;

namespace BlowinCleanCode.Extension
{
    public static class ExpressionSyntaxExt
    {
        private static readonly (StringSegment, IMatcher<StringSegment>)[] CreationMethodPatternNames = 
        {
            (new StringSegment("Open"), StartWithMatcher.InstanceInvariantCultureIgnoreCase),
            (new StringSegment("From"), StartWithMatcher.InstanceInvariantCultureIgnoreCase),
            (new StringSegment("Build"), StartWithMatcher.InstanceInvariantCultureIgnoreCase),
            (new StringSegment("Create"), StartWithMatcher.InstanceInvariantCultureIgnoreCase),
            (new StringSegment("Clone"), StartWithMatcher.InstanceInvariantCultureIgnoreCase),
            (new StringSegment("Of"), StringEqualityMatcher.InstanceInvariantCultureIgnoreCase),
        };
        
        public static bool IsCreation(this ExpressionSyntax self)
        {
            if (self is ConditionalExpressionSyntax conditional)
                return IsCreationCore(conditional.WhenTrue) || IsCreationCore(conditional.WhenFalse);
            
            return IsCreationCore(self);
        }

        private static bool IsCreationCore(ExpressionSyntax self)
        {
            if (self == null)
                return false;

            if (self is InvocationExpressionSyntax invocation && invocation.Expression != null)
            {
                var methodName = new StringSegment(invocation.Expression.ToString());
                var lastDot = methodName.LastIndexOf('.');
                if (lastDot >= 0)
                    methodName = methodName.Subsegment(lastDot + 1);
                
                return methodName.MatchAny(CreationMethodPatternNames);
            }

            return self is ObjectCreationExpressionSyntax;
        }
    }
}