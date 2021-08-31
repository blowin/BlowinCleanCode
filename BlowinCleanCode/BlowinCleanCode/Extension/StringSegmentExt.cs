using System;
using Microsoft.Extensions.Primitives;

namespace BlowinCleanCode.Extension
{
    public static class StringSegmentExt
    {
        public static bool StartsWith(this StringSegment self, StringSegment text, StringComparison comparisonType)
        {
            var selfLen = self.Length;
            var textLen = text.Length;

            if (selfLen < textLen)
                return false;
            
            var selfBuffer = self.Buffer ?? string.Empty;
            var selfOffset = self.Offset;
            
            var textBuffer = text.Buffer ?? string.Empty;
            var textOffset = text.Offset;

            return string.Compare(selfBuffer, selfOffset, textBuffer, textOffset, textLen, comparisonType) == 0;
        }
    }
}