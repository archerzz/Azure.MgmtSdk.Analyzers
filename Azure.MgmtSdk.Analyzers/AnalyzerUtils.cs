using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace Azure.MgmtSdk.Analyzers
{
    internal class AnalyzerUtils
    {
        internal static bool IsNotSdkCode(ISymbol symbol) => !IsSdkCode(symbol);

        internal static bool IsSdkCode(ISymbol symbol)
        {
            var ns = symbol.ContainingNamespace.GetFullNamespaceName();

            return IsSdkNamespace(ns);
        }

        internal static bool IsNotSdkCode(SyntaxNode node, SemanticModel model) => !IsSdkCode(node, model);

        internal static bool IsSdkCode(SyntaxNode node, SemanticModel model)
        {
            var symbol = model.GetDeclaredSymbol(node);
            if (symbol != null)
            {
                return IsSdkCode(symbol);
            }

            var ns = GetNamespace(node);
            return IsSdkNamespace(ns);
        }

        private static bool IsSdkNamespace(string ns)
        {
            var namespaces = ns.Split('.');
            if (namespaces.Length >= 2)
            {
                return namespaces[0].Equals("Azure", StringComparison.OrdinalIgnoreCase) &&
                    namespaces[1].Equals("ResourceManager", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private static string GetNamespace(SyntaxNode node)
        {
            var ns = string.Empty;

            var parent = node.Parent;

            while (parent != null &&
                    parent is not NamespaceDeclarationSyntax
                    && parent is not FileScopedNamespaceDeclarationSyntax)
            {
                parent = parent.Parent;
            }

            if (parent is BaseNamespaceDeclarationSyntax namespaceParent)
            {
                ns = namespaceParent.Name.ToString();

                while (true)
                {
                    if (namespaceParent.Parent is not NamespaceDeclarationSyntax parentNamespace)
                    {
                        break;
                    }

                    ns = $"{namespaceParent.Name}.{ns}";
                    namespaceParent = parentNamespace;
                }
            }

            return ns;
        }
    }
}
