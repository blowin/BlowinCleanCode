using System;
using System.Collections;
using System.Collections.Generic;
using BlowinCleanCode.Model;

namespace BlowinCleanCode.Extension
{
    public static class StringExt
    {
        public static bool IsAscii(this string self)
        {
            if(string.IsNullOrEmpty(self))
                return false;
            
            for (var i = 0; i < self.Length; i++)
            {
                if (!self[i].IsAscii())
                    return false;
            }

            return true;
        }

        public static IEnumerable<StringSlice> SplitEnumerator(this string self, string splitValue, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            var idx = 0;
            do
            {
                var oldIdx = idx;
                idx = self.IndexOf(splitValue, idx, comparison);
                if (idx >= 0)
                {
                    yield return new StringSlice(oldIdx, idx - oldIdx, self);
                    idx += splitValue.Length;

                    if (idx >= self.Length)
                        yield return StringSlice.Empty;
                }
                else
                {
                    yield return new StringSlice(oldIdx, self.Length - oldIdx, self);
                }

            }
            while (idx > 0 && idx < self.Length);
        }
    }
}