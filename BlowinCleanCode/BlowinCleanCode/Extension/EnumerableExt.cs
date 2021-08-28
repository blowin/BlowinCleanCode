using System.Collections.Generic;

namespace BlowinCleanCode.Extension
{
    public static class EnumerableExt
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> self) => new HashSet<T>(self);

        public static IEnumerable<T> ToSingleEnumerable<T>(this T self)
        {
            yield return self;
        }
    }
}