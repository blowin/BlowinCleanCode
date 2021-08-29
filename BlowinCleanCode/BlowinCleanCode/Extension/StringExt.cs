namespace BlowinCleanCode.Extension
{
    public static class StringExt
    {
        /// <summary>
        /// Проверяет, что <see cref="self"/> начинается на один из элементов <see cref="checkArrayItems"/> 
        /// </summary>
        public static bool StartWithAny(this string self, string[] checkArrayItems)
        {
            foreach (var str in checkArrayItems)
            {
                if (self.StartsWith(str))
                    return true;
            }

            return false;
        }
    }
}