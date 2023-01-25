using Microsoft.CodeAnalysis;

namespace BlowinCleanCode.Extension
{
    public static class AttributeDataExt
    {
        public static bool IsThreadStatic(this AttributeData self) => self.ToString() == "System.ThreadStaticAttribute";
    }
}
