using System;

namespace BlowinCleanCode.Model.Matchers
{
    public sealed class StartWithMatcher : IMatcher<string>
    {
        private readonly StringComparison _comparison;

        public static StartWithMatcher InstanceInvariantCultureIgnoreCase { get; } = new StartWithMatcher(StringComparison.InvariantCultureIgnoreCase);
        
        public StartWithMatcher(StringComparison comparison) => _comparison = comparison;

        public bool Match(string left, string right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (left == null || right == null)
                return false;
            
            return left.StartsWith(right, _comparison);
        }
    }
}