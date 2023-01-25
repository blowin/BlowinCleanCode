using System;
using System.Runtime.CompilerServices;

// ReSharper disable LocalizableElement
namespace BlowinCleanCode.Model
{
    public readonly struct StringSlice : IEquatable<StringSlice>
    {
        public static StringSlice Empty => new StringSlice(string.Empty);

        public int Start
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public string Data
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Length == 0;
        }

        public char this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Data[Start + index];
        }

        public StringSlice(string data)
            : this(data, 0)
        {
        }

        public StringSlice(string data, int start)
            : this(data, start, data.Length - start)
        {
        }

        public StringSlice(string data, int start, int length)
        {
            if (data == null)
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

        public StringSlice Substring(int startIndex)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, "The start index must be nonnegative.");
            if (startIndex > Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, "The start index must not fall out of the string length.");
            return startIndex == 0 ? this : new StringSlice(Data, Start + startIndex, Length - startIndex);
        }

        public StringSlice Substring(int startIndex, int length)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, "The start index must be nonnegative.");
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, "The length must be nonnegative.");
            if (startIndex + length > Length)
                throw new ArgumentOutOfRangeException(nameof(length), length, "The length must not fall out of the string.");
            return new StringSlice(Data, Start + startIndex, Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StringSlice TrimStart()
        {
            for (var i = 0; i < Length; i++)
            {
                if (char.IsWhiteSpace(this[i]))
                    continue;

                return new StringSlice(Data, Start + i, Length - i);
            }

            return Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StringSlice TrimEnd()
        {
            for (var i = Length - 1; i >= 0; i--)
            {
                if (char.IsWhiteSpace(this[i]))
                    continue;

                return new StringSlice(Data, Start, i + 1);
            }

            return Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StringSlice Trim() => TrimStart().TrimEnd();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(StringSlice other, StringComparison comparison) => Length == other.Length && string.Compare(Data, Start, other.Data, other.Start, Length, comparison) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(StringSlice other) => Equals(other, StringComparison.InvariantCulture);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) => obj is StringSlice other && Equals(other);

        public bool Equals(string value, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase) => Length == value.Length && string.Compare(Data, Start, value, 0, value.Length, comparison) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => IsEmpty ? string.Empty : Data.Substring(Start, Length);
    }
}
