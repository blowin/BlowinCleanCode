using System.Runtime.CompilerServices;

namespace BlowinCleanCode.Extension
{
    public static partial class ObjectExt
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
    }
}