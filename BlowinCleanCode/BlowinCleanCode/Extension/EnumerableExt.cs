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

        /// <summary>
        /// If has no elements, tuple = null
        /// If has less, than 2 value, than second = default
        /// otherwise first = firstElement, second = secondElement
        ///
        /// For example:
        ///     input seq [] = null
        ///     input seq ["Hello"] = ("Hello", null)
        ///     input seq ["Hello", "World"] = ("Hello", "World")
        ///     input seq ["Hello", "World", "Third", ...] = ("Hello", "World")
        /// </summary>
        /// <returns></returns>
        public static (T first, T second)? FirstPairOrDefault<T>(this IEnumerable<T> self)
        {
            T first = default;
            T second = default;
            int number = 0;
            foreach (var item in self)
            {
                number += 1;
                if (number == 1)
                {
                    first = item;
                }
                else if (number == 2)
                {
                    second = item;
                }
                else
                {
                    break;
                }
            }

            if (number == 0)
                return null;

            return (first, second);
        }
    }
}