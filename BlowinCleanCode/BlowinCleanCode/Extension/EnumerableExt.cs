using System.Collections.Generic;
using BlowinCleanCode.Model;

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
        ///     input seq ["Hello", "World", "Third", ...] = ("Hello", "World").
        /// </summary>
        /// <typeparam name="T">Type of elements.</typeparam>
        /// <param name="self">Sequence.</param>
        /// <returns>
        ///     First pair of sequence.
        /// </returns>
        public static (Optional<T> first, Optional<T> second) FirstPairOrDefault<T>(this IEnumerable<T> self)
        {
            var first = Optional<T>.None();
            var second = Optional<T>.None();
            foreach (var item in self)
            {
                if (!first.HasValue)
                {
                    first = Optional<T>.Some(item);
                }
                else if (!second.HasValue)
                {
                    second = Optional<T>.Some(item);
                }
                else
                {
                    break;
                }
            }

            return (first, second);
        }
    }
}
