using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BlowinCleanCode.Model.Matchers;

namespace BlowinCleanCode.Extension
{
    public static class ObjectExt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAny<T1, T2, T3, T4, T5, T6, T7>(this object self) => self is T1 || self is T2 || self is T3 || self is T4 || self is T5 || self is T6 || self is T7;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAny<T1, T2, T3, T4, T5, T6>(this object self) => self is T1 || self is T2 || self is T3 || self is T4 || self is T5 || self is T6;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAny<T1, T2, T3, T4, T5>(this object self) => self is T1 || self is T2 || self is T3 || self is T4 || self is T5;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAny<T1, T2, T3, T4>(this object self) => self is T1 || self is T2 || self is T3 || self is T4;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAny<T1, T2, T3>(this object self) => self is T1 || self is T2 || self is T3;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAny<T1, T2>(this object self) => self is T1 || self is T2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is<T>(this object self) => self is T;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNot<T>(this object self) => !self.Is<T>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is<T>(this object self, out T value)
        {
            if (self is T res)
            {
                value = res;
                return true;
            }

            value = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool MatchAny<T>(this T self, (T CheckValue, IMatcher<T> Matcher)[] checkArrayItems)
        {
            foreach (var (str, matcher) in checkArrayItems)
            {
                if (matcher.Match(self, str))
                    return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool In<T>(this T self, T v1, T v2, T v3, T v4, T v5, T v6)
        {
            if (typeof(T).IsValueType)
            {
                return EqualityComparer<T>.Default.Equals(self, v1) ||
                       EqualityComparer<T>.Default.Equals(self, v2) ||
                       EqualityComparer<T>.Default.Equals(self, v3) ||
                       EqualityComparer<T>.Default.Equals(self, v4) ||
                       EqualityComparer<T>.Default.Equals(self, v5) ||
                       EqualityComparer<T>.Default.Equals(self, v6);
            }

            var comparer = EqualityComparer<T>.Default;
            return comparer.Equals(self, v1) ||
                   comparer.Equals(self, v2) ||
                   comparer.Equals(self, v3) ||
                   comparer.Equals(self, v4) ||
                   comparer.Equals(self, v5) ||
                   comparer.Equals(self, v6);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool In<T>(this T self, T v1, T v2, T v3, T v4, T v5)
        {
            if (typeof(T).IsValueType)
            {
                return EqualityComparer<T>.Default.Equals(self, v1) ||
                       EqualityComparer<T>.Default.Equals(self, v2) ||
                       EqualityComparer<T>.Default.Equals(self, v3) ||
                       EqualityComparer<T>.Default.Equals(self, v4) ||
                       EqualityComparer<T>.Default.Equals(self, v5);
            }

            var comparer = EqualityComparer<T>.Default;
            return comparer.Equals(self, v1) ||
                   comparer.Equals(self, v2) ||
                   comparer.Equals(self, v3) ||
                   comparer.Equals(self, v4) ||
                   comparer.Equals(self, v5);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool In<T>(this T self, T v1, T v2, T v3, T v4)
        {
            if (typeof(T).IsValueType)
            {
                return EqualityComparer<T>.Default.Equals(self, v1) ||
                       EqualityComparer<T>.Default.Equals(self, v2) ||
                       EqualityComparer<T>.Default.Equals(self, v3) ||
                       EqualityComparer<T>.Default.Equals(self, v4);
            }

            var comparer = EqualityComparer<T>.Default;
            return comparer.Equals(self, v1) ||
                   comparer.Equals(self, v2) ||
                   comparer.Equals(self, v3) ||
                   comparer.Equals(self, v4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool In<T>(this T self, T v1, T v2, T v3)
        {
            if (typeof(T).IsValueType)
                return EqualityComparer<T>.Default.Equals(self, v1) || EqualityComparer<T>.Default.Equals(self, v2) || EqualityComparer<T>.Default.Equals(self, v3);

            var comparer = EqualityComparer<T>.Default;
            return comparer.Equals(self, v1) || comparer.Equals(self, v2) || comparer.Equals(self, v3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool In<T>(this T self, T v1, T v2)
        {
            if (typeof(T).IsValueType)
                return EqualityComparer<T>.Default.Equals(self, v1) || EqualityComparer<T>.Default.Equals(self, v2);

            var comparer = EqualityComparer<T>.Default;
            return comparer.Equals(self, v1) || comparer.Equals(self, v2);
        }
    }
}
