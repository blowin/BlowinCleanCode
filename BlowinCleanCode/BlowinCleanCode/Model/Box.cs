using System;
using System.Collections.Generic;

namespace BlowinCleanCode.Model
{
    public sealed class Box<T> : IEquatable<T>, IEquatable<Box<T>>
        where T : struct
    {
        public T Value { get; set; }

        public Box(T value) => Value = value;

        public bool Equals(Box<T> other) => other != null && Equals(other.Value);

        public bool Equals(T other) => EqualityComparer<T>.Default.Equals(Value, other);

        public override bool Equals(object obj) => ReferenceEquals(this, obj) || obj is Box<T> other && Equals(other);

        public override int GetHashCode() => EqualityComparer<T>.Default.GetHashCode(Value);

        public override string ToString() => $"Box({Value})";
    }

    public static class Box
    {
        public static Box<T> From<T>(T from) where T : struct => new Box<T>(from);
    }
}