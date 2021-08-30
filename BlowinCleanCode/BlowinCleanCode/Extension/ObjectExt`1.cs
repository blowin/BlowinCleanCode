using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BlowinCleanCode.Model.Matchers;

namespace BlowinCleanCode.Extension
{
    public static partial class ObjectExt
    {
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
            if(typeof(T).IsValueType)
                return EqualityComparer<T>.Default.Equals(self, v1) || 
                       EqualityComparer<T>.Default.Equals(self, v2) || 
                       EqualityComparer<T>.Default.Equals(self, v3) || 
                       EqualityComparer<T>.Default.Equals(self, v4) || 
                       EqualityComparer<T>.Default.Equals(self, v5) || 
                       EqualityComparer<T>.Default.Equals(self, v6);

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
            if(typeof(T).IsValueType)
                return EqualityComparer<T>.Default.Equals(self, v1) || 
                       EqualityComparer<T>.Default.Equals(self, v2) || 
                       EqualityComparer<T>.Default.Equals(self, v3) || 
                       EqualityComparer<T>.Default.Equals(self, v4) || 
                       EqualityComparer<T>.Default.Equals(self, v5);

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
            if(typeof(T).IsValueType)
                return EqualityComparer<T>.Default.Equals(self, v1) || 
                       EqualityComparer<T>.Default.Equals(self, v2) || 
                       EqualityComparer<T>.Default.Equals(self, v3) || 
                       EqualityComparer<T>.Default.Equals(self, v4);

            var comparer = EqualityComparer<T>.Default;
            return comparer.Equals(self, v1) || 
                   comparer.Equals(self, v2) || 
                   comparer.Equals(self, v3) || 
                   comparer.Equals(self, v4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool In<T>(this T self, T v1, T v2, T v3)
        {
            if(typeof(T).IsValueType)
                return EqualityComparer<T>.Default.Equals(self, v1) || EqualityComparer<T>.Default.Equals(self, v2) || EqualityComparer<T>.Default.Equals(self, v3);

            var comparer = EqualityComparer<T>.Default;
            return comparer.Equals(self, v1) || comparer.Equals(self, v2) || comparer.Equals(self, v3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool In<T>(this T self, T v1, T v2)
        {
            if(typeof(T).IsValueType)
                return EqualityComparer<T>.Default.Equals(self, v1) || EqualityComparer<T>.Default.Equals(self, v2);

            var comparer = EqualityComparer<T>.Default;
            return comparer.Equals(self, v1) || comparer.Equals(self, v2);
        }
    }
}