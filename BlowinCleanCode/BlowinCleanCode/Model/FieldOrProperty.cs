using System;
using System.Collections.Immutable;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Extension.SymbolExtension;
using Microsoft.CodeAnalysis;

namespace BlowinCleanCode.Model
{
    public readonly struct FieldOrProperty : IEquatable<FieldOrProperty>
    {
        public bool IsField => Field != null;

        public string Name => Field?.NormalizeName() ?? Property.NormalizeName();

        public ITypeSymbol Type => IsField ? Field.Type : Property?.Type;

        public bool IsBackingField => IsField && Field.IsBackingField();

        public ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get
            {
                if (IsField)
                    return Field.DeclaringSyntaxReferences;

                return Property?.DeclaringSyntaxReferences ?? ImmutableArray<SyntaxReference>.Empty;
            }
        }

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
                return ((Field != null ? SymbolEqualityComparer.Default.GetHashCode(Field) : 0) * 397) ^ (Property != null ? SymbolEqualityComparer.Default.GetHashCode(Property) : 0);
            }
        }
    }
}