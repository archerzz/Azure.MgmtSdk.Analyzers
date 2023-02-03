using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System.Text;
using System.Data;

namespace Azure.MgmtSdk.Analyzers
{
    /// <summary>
    /// Analyzer to check type name suffixes. This file contains the base class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ModelNameSuffixAnalyzerBase : DiagnosticAnalyzer
    {
        protected static readonly string Title = "Improper model name suffix";
        protected static readonly string MessageFormat = "Model name '{0}' ends with '{1}'. Suggest to rename it as {2}.";
        protected static readonly string Description = "Suffix is not recommended. Consider to remove or modify it.";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray<DiagnosticDescriptor>.Empty;

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
        }

        protected bool IsClass(ITypeSymbol symbol) => symbol is { TypeKind: TypeKind.Class };

        protected bool HasModelsNamespace(INamedTypeSymbol typeSymbol)
        {
            bool hasNamespaceModels = false;
            for (var namespaceSymbol = typeSymbol.ContainingNamespace; namespaceSymbol != null; namespaceSymbol = namespaceSymbol.ContainingNamespace)
            {
                var fullNamespace = namespaceSymbol.Name;
                if (fullNamespace.Split('.').Any(name => name.Equals("Models")))
                {
                    hasNamespaceModels = true;
                    break;
                }
            }
            return hasNamespaceModels;
        }

        protected bool ImplementsInterfaceOrClass(INamedTypeSymbol typeSymbol, string namespaceName, string typeName)
        {
            if (typeSymbol is null)
                return false;

            // check class hierachy
            INamedTypeSymbol classType = typeSymbol;
            while (classType is not null)
            {
                if (classType.MetadataName == typeName && classType.ContainingNamespace.GetFullNamespaceName() == namespaceName)
                    return true;
                classType = classType.BaseType;
            };

            // check interfaces
            return typeSymbol.AllInterfaces.Any(@interface => @interface.MetadataName == typeName && @interface.ContainingNamespace.Name == namespaceName);
        }

        private string GetFullNamespace(INamespaceSymbol namespaceSymbol)
        {
            if (namespaceSymbol is { ContainingNamespace: not null and { IsGlobalNamespace: false} })
            {
                return GetFullNamespace(namespaceSymbol.ContainingNamespace) + "." + namespaceSymbol.Name;
            }

            return namespaceSymbol.Name;
        }
    }
}
