using Microsoft.CodeAnalysis;

namespace BlowinCleanCode.Extension
{
    public static class NamedTypeExt
    {
        public static bool IsDisposableOrAsyncDisposable(this INamedTypeSymbol self) =>
            self.IsDisposable() || self.IsAsyncDisposable();
        
        public static bool IsDisposable(this INamedTypeSymbol self) =>
            self.SpecialType == SpecialType.System_IDisposable;

        public static bool IsAsyncDisposable(this INamedTypeSymbol self) =>
            self.ContainingModule?.Name == "System.Runtime.dll" &&
            self.Name == "IAsyncDisposable";
    }
}