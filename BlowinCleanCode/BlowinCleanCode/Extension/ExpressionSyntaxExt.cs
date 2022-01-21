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
            ("Clone", StartWithMatcher.InstanceInvariantCultureIgnoreCase),
            ("Of", StringEqualityMatcher.InstanceInvariantCultureIgnoreCase),
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
                var methodName = invocation.Expression.ToString();
                var lastDot = methodName.LastIndexOf('.');
                if (lastDot >= 0)
                    methodName = methodName.Substring(lastDot + 1);
                
                return methodName.MatchAny(CreationMethodPatternNames);
            }

            return self is ObjectCreationExpressionSyntax;
        }
    }
}