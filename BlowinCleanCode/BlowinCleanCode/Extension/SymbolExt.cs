﻿using Microsoft.CodeAnalysis;

namespace BlowinCleanCode.Extension
{
    public static class SymbolExt
    {
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