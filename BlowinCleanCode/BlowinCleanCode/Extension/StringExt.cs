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
    }
}