using System;
using Microsoft.Extensions.Primitives;

namespace BlowinCleanCode.Model.Matchers
{
    public sealed class StringEqualityMatcher : IMatcher<StringSegment>
    {
        public static StringEqualityMatcher InstanceInvariantCultureIgnoreCase { get; } = new StringEqualityMatcher();

        public bool Match(StringSegment left, StringSegment right)
        {
            return left.Equals(right, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}