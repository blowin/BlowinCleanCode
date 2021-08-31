using System;
using BlowinCleanCode.Extension;
using Microsoft.Extensions.Primitives;

namespace BlowinCleanCode.Model.Matchers
{
    public sealed class StartWithMatcher : IMatcher<StringSegment>
    {
        private readonly StringComparison _comparison;

        public static StartWithMatcher InstanceInvariantCultureIgnoreCase { get; } = new StartWithMatcher(StringComparison.InvariantCultureIgnoreCase);
        
        public StartWithMatcher(StringComparison comparison) => _comparison = comparison;

        public bool Match(StringSegment left, StringSegment right)
        {
            if (left.Equals(right))
                return true;
            
            return left.StartsWith(right, _comparison);
        }
    }
}