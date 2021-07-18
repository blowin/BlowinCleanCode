using System.Collections.Generic;

namespace BlowinCleanCode.Extension
{
    public static class EnumerableExt
    {
        public static IEnumerable<T> ToSingleEnumerable<T>(this T self)
        {
            yield return self;
        }
    }
}