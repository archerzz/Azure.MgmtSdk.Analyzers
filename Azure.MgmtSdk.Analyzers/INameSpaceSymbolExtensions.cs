
using Microsoft.CodeAnalysis;

namespace Azure.MgmtSdk.Analyzers
{
    public static class INamespaceSymbolExtensions
    {
        public static string GetFullNamespaceName(this INamespaceSymbol symbol)
        {
            if (symbol is { ContainingNamespace: not null and { IsGlobalNamespace: false} })
            {
                return $"{symbol.ContainingNamespace.GetFullNamespaceName()}.{symbol.Name}";
            }

            return symbol.Name;
        }
    }
}
