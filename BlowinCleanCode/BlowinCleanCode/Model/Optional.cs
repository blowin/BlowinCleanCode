using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BlowinCleanCode.Model
{
    public readonly struct Optional<T> : IEquatable<Optional<T>>, IEquatable<T>
    {
        private readonly T _value;
        private readonly bool _hasValue;

        public T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _value;
        }

        public bool HasValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _hasValue;
        }

        public Optional(T value)
            : this(value, true)
        {
        }

        private Optional(T value, bool hasValue)
        {
            _value = value;
            _hasValue = hasValue;
        }

        public static Optional<T> None() => new Optional<T>(default, false);

        public static Optional<T> Some(T value) => new Optional<T>(value);

        public bool Equals(Optional<T> other) => _hasValue == other._hasValue && EqualityComparer<T>.Default.Equals(_value, other._value);

        public bool Equals(T other) => _hasValue && EqualityComparer<T>.Default.Equals(_value, other);

        public override bool Equals(object obj) => (obj is Optional<T> optional && Equals(optional)) || (obj is T value && Equals(value));

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T>.Default.GetHashCode(_value) * 397) ^ _hasValue.GetHashCode();
            }
        }

        public override string ToString() => HasValue ? $"Optional({Value})" : "Optional(None)";
    }
}
