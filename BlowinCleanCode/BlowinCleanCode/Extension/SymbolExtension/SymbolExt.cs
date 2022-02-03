using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlowinCleanCode.Extension.SymbolExtension
{
    public static class SymbolExt
    {
        public static IEnumerable<T> AsSyntax<T>(this IEnumerable<ISymbol> self)
            where T : CSharpSyntaxNode
        {
            return self
                .SelectMany(e => e.DeclaringSyntaxReferences.Select(dsr => dsr.GetSyntax() as T))
                .Where(e => e != null);
        }

        public static bool IsBackingField(this ISymbol symbol)
        {
            var name = symbol.Name ?? string.Empty;
            return name.StartsWith("<") && name.IndexOf('>', 1) >= 0;
        }
        
        /// <summary>
        /// For background property field return property name
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static string NormalizeName(this ISymbol symbol)
        {
            var name = symbol.Name ?? string.Empty;
            if (!name.StartsWith("<"))
                return name;

            var end = name.IndexOf('>', 1);
            return end < 0 ? name : name.Substring(1, end - 1);
        }
    }
}