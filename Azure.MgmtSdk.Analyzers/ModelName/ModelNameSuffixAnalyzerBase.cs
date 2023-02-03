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

namespace Azure.MgmtSdk.Analyzers.ModelName
{
    /// <summary>
    /// Analyzer to check type name suffixes. This file contains the base class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public abstract class ModelNameSuffixAnalyzerBase : DiagnosticAnalyzer
    {
        protected static readonly string Title = "Improper model name suffix";
        protected static readonly string Description = "Suffix is not recommended. Consider to remove or modify it.";
        protected static readonly string GeneralRenamingMessageFormat = "Model name '{0}' ends with '{1}'. Suggest to rename it to an appropriate name.";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray<DiagnosticDescriptor>.Empty;

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
        }

        private void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var typeSymbol = (INamedTypeSymbol)context.Symbol;
            if (!IsClassUnderModelsNamespace(typeSymbol) || ShouldSkip(typeSymbol))
                return;

            var name = typeSymbol.Name;
            var match = MatchRegex.Match(name);
            if (match.Success)
            {
                var suffix = match.Groups["Suffix"].Value;
                context.ReportDiagnostic(GetDiagnostic(typeSymbol, suffix, context));
            }
        }

        protected virtual bool ShouldSkip(INamedTypeSymbol symbol) => false;
        protected virtual Regex MatchRegex => throw new NotImplementedException();
        protected virtual Diagnostic GetDiagnostic(INamedTypeSymbol typeSymbol, string suffix, SymbolAnalysisContext context) => throw new NotImplementedException();

        protected bool IsTypeOf(INamedTypeSymbol typeSymbol, string namespaceName, string typeName)
        {
            if (typeSymbol is null)
                return false;

            // check class hierachy
            INamedTypeSymbol classType = typeSymbol;
            while (classType is not null)
            {
                if (classType.Name == typeName && classType.ContainingNamespace.GetFullNamespaceName() == namespaceName)
                    return true;
                classType = classType.BaseType;
            };

            // check interfaces
            return typeSymbol.AllInterfaces.Any(@interface => @interface.Name == typeName && @interface.ContainingNamespace.Name == namespaceName);
        }

        private bool IsClassUnderModelsNamespace(ITypeSymbol symbol) => symbol is { TypeKind: TypeKind.Class } && HasModelsNamespace(symbol);

        private bool HasModelsNamespace(ITypeSymbol typeSymbol)
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

        private string GetFullNamespace(INamespaceSymbol namespaceSymbol)
        {
            if (namespaceSymbol is { ContainingNamespace: not null and { IsGlobalNamespace: false } })
            {
                return GetFullNamespace(namespaceSymbol.ContainingNamespace) + "." + namespaceSymbol.Name;
            }

            return namespaceSymbol.Name;
        }
    }
}
