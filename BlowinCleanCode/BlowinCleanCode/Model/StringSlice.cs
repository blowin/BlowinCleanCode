using System;
using System.Runtime.CompilerServices;

namespace BlowinCleanCode.Model
{
    public readonly struct StringSlice : IEquatable<StringSlice>
    {
        public static StringSlice Empty => new StringSlice(string.Empty);

        public int Start { [MethodImpl(MethodImplOptions.AggressiveInlining)]get; }
        public int Length { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }
        public string Data { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }
        public bool IsEmpty { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Length == 0; }

        public StringSlice(string data) : this(0, data.Length, data)
        {
        }

        public StringSlice(int start, int length, string data)
        {
            if(data == null)
                throw new ArgumentNullException(nameof(data));
            if (start < 0)
                throw new ArgumentOutOfRangeException(nameof(start), start, "The range start index must be nonnegative.");
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, "The range length must be nonnegative.");
            if (start + length > data.Length)
                throw new ArgumentOutOfRangeException(nameof(length), length, "The length must not fall out of the string.");
            Start = start;
            Length = length;
            Data = data;
        }

        public bool Equals(StringSlice other) => Start == other.Start && Length == other.Length && Data == other.Data;

        public override bool Equals(object obj) => obj is StringSlice other && Equals(other);

        public bool Equals(string value, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase) => Length == value.Length && string.Compare(Data, Start, value, 0, value.Length, comparison) == 0;

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Start;
                hashCode = (hashCode * 397) ^ Length;
                hashCode = (hashCode * 397) ^ (Data != null ? Data.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString() => Data.Substring(Start, Length);
    }
}