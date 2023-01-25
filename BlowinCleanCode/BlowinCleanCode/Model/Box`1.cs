using System;
using System.Collections.Generic;

namespace BlowinCleanCode.Model
{
#pragma warning disable SA1649 // File name should match first type name
    public sealed class Box<T> : IEquatable<T>, IEquatable<Box<T>>
#pragma warning restore SA1649 // File name should match first type name
        where T : struct
    {
        public T Value { get; set; }

        public Box(T value) => Value = value;

        public bool Equals(Box<T> other) => other != null && Equals(other.Value);

        public bool Equals(T other) => EqualityComparer<T>.Default.Equals(Value, other);

        public override bool Equals(object obj) => ReferenceEquals(this, obj) || (obj is Box<T> other && Equals(other));

        public override int GetHashCode() => EqualityComparer<T>.Default.GetHashCode(Value);

        public override string ToString() => $"Box({Value})";
    }
}
