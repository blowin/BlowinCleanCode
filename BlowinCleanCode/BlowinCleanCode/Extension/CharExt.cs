namespace BlowinCleanCode.Extension
{
    public static class CharExt
    {
        public static bool IsAscii(this char self)
        {
            const int minAsciiCode = 32;
            const int maxAsciiCode = 127;
            return self >= minAsciiCode && self < maxAsciiCode;
        }
    }
}