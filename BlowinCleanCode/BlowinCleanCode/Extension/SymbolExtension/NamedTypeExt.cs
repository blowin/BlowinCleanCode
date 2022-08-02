using System.Collections.Generic;
using System.Linq;
using BlowinCleanCode.Model;
using Microsoft.CodeAnalysis;

namespace BlowinCleanCode.Extension.SymbolExtension
{
    public static class NamedTypeExt
    {
        public static IEnumerable<FieldOrProperty> FieldOrProperties(this INamedTypeSymbol symbol, bool includeBackingField = false)
        {
            foreach (var member in symbol.GetMembers())
            {
                if(!FieldOrProperty.IsFieldOrProperty(member))
                    continue;

                var fieldOrProperty = FieldOrProperty.Create(member);
                if(!includeBackingField && fieldOrProperty.IsBackingField)
                    continue;

                yield return fieldOrProperty;
            }
        }

        public static bool IsDisposableOrAsyncDisposable(this INamedTypeSymbol self) =>
            self.IsDisposable() || self.IsAsyncDisposable();
        
        public static bool IsDisposable(this INamedTypeSymbol self) =>
            self.SpecialType == SpecialType.System_IDisposable;

        public static bool IsAsyncDisposable(this INamedTypeSymbol self) =>
            self.ContainingModule?.Name == "System.Runtime.dll" &&
            self.Name == "IAsyncDisposable";
        
        public static bool HasUserMethods(this INamedTypeSymbol self)
        {
            return self.Methods(false).Any();
        }

        public static IEnumerable<IMethodSymbol> Methods(this INamedTypeSymbol self, bool includeSystem)
        {
            foreach (var member in self.GetMembers())
            {
                if(!member.Is<IMethodSymbol>(out var methodSymbol))
                    continue;

                if (methodSymbol.MethodKind != MethodKind.Ordinary) 
                    continue;

                if (!includeSystem && methodSymbol.Name.In(nameof(Equals), nameof(GetHashCode), nameof(ToString))) 
                    continue;
                
                yield return methodSymbol;
            }
        }

        public static IEnumerable<IFieldSymbol> Fields(this INamedTypeSymbol self)
            => self.GetMembers().OfType<IFieldSymbol>();
    }
}