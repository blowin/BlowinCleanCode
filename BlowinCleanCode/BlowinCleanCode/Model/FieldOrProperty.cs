﻿using System;
using BlowinCleanCode.Extension;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Primitives;

namespace BlowinCleanCode.Model
{
    public readonly struct FieldOrProperty : IEquatable<FieldOrProperty>
    {
        public bool IsField => Field != null;

        public StringSegment Name => Field?.NormalizeName() ?? Property.NormalizeName();
            
        public IFieldSymbol Field { get; }
        public IPropertySymbol Property { get; }
            
        public FieldOrProperty(IFieldSymbol field)
        {
            Field = field;
            Property = default;
        }

        public FieldOrProperty(IPropertySymbol property)
        {
            Field = default;
            Property = property;
        }

        public static bool IsFieldOrProperty(ISymbol symbol) => symbol is IFieldSymbol || symbol is IPropertySymbol;
        
        public static FieldOrProperty Create(ISymbol symbol) => symbol.Is<IFieldSymbol>(out var f)
            ? new FieldOrProperty(f)
            : new FieldOrProperty((IPropertySymbol)symbol);
        
        public bool Equals(FieldOrProperty other)
        {
            if (Field != null && SymbolEqualityComparer.Default.Equals(Field, other.Field))
                return true;

            if (Property != null && SymbolEqualityComparer.Default.Equals(Property, other.Property))
                return true;
                
            return false;
        }

        public override bool Equals(object obj) => obj is FieldOrProperty other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Field != null ? Field.GetHashCode() : 0) * 397) ^ (Property != null ? Property.GetHashCode() : 0);
            }
        }
    }
}