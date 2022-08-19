using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Azure.MgmtSdk.Analyzers
{
    /// <summary>
    /// Analyzer to check type name suffixes. This file contains the base class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ModelNameSuffixAnalyzerBase : DiagnosticAnalyzer
    {
        protected static readonly string Title = "Improper model name suffix";
        protected static readonly string MessageFormat = "Model name '{0}' ends with '{1}'";
        protected static readonly string Description = "Suffix is not recommended. Consider to remove or modify.";

        //public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => throw new NotImplementedException();
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray<DiagnosticDescriptor>.Empty;

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
        }

        protected bool IsClass(INamedTypeSymbol typeSymbol)
        {
            if (typeSymbol.TypeKind != TypeKind.Class)
                return false;
            return true;
        }

        protected bool HasModelsNamespace(INamedTypeSymbol typeSymbol)
        {
            bool hasNamespaceModels = false;
            for (var namespaceSymbol = typeSymbol.ContainingNamespace; namespaceSymbol != null; namespaceSymbol = namespaceSymbol.ContainingNamespace)
            {
                var fullNamespace = namespaceSymbol.Name;
                if (fullNamespace.Contains("Models"))
                {
                    hasNamespaceModels = true;
                    break;
                }
            }
            return hasNamespaceModels;
        }
        protected bool ImplementsInterfaceOrBaseClass(INamedTypeSymbol typeSymbol, string nameToCheck)
        {
            if (typeSymbol == null)
                return false;

            // judge base classes
            if (typeSymbol.MetadataName == nameToCheck)
                return true;

            INamedTypeSymbol tempSymbol = typeSymbol;
            while (tempSymbol.BaseType != null)
            {
                tempSymbol = tempSymbol.BaseType;
                if (tempSymbol.MetadataName == nameToCheck)
                    return true;
            }

            // judge interfaces
            foreach (var @interface in typeSymbol.AllInterfaces)
                if (@interface.MetadataName == nameToCheck)
                    return true;

            return false;
        }
    }
}
