using System;

namespace BlowinCleanCode.Model.Matchers
{
    public sealed class StringEqualityMatcher : IMatcher<string>
    {
        public static StringEqualityMatcher InstanceInvariantCultureIgnoreCase { get; } = new StringEqualityMatcher();

        public bool Match(string left, string right)
        {
            if (left == null && right == null)
                return true;

            if (left == null || right == null)
                return false;

            return left.Equals(right, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}