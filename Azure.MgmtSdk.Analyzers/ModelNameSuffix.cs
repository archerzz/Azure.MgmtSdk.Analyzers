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
    /// Analyzer to check type name suffixes. There are some suffixed we don't recommend to use, like `Result`, `Response`...
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ModelNameSuffixAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticIdBase = "AZM0010";

        protected static readonly string Title = "Imroper model name suffix";
        protected static readonly string MessageFormat = "Model name '{0}' ends with '{1}'";
        protected static readonly string Description = "Suffix is not recommended. Consider to remove or modify.";

        public static DiagnosticDescriptor AZM0010 = new DiagnosticDescriptor(
            nameof(AZM0010),
            Title,
            MessageFormat, 
            DiagnosticCategory.Naming, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true,
            description: Description);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(AZM0010); } }

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

    public class ModelNameSuffixBasicAnalyzer : ModelNameSuffixAnalyzer
    {
        public const string DiagnosticIdBasic = DiagnosticIdBase + "C0";

        private static readonly HashSet<string> ReservedNames = new HashSet<string> { "ErrorResponse" };

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIdBase, Title,
            MessageFormat, DiagnosticCategory.Naming, DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: Description);

        // Model suffix forbidden
        private static readonly Regex SuffixRegex = new Regex(".+(?<Suffix>(Results?)|(Requests?)|(Responses?)|(Parameters?)|(Options?)|(Collection)|(Resource))$");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(SymbolAnalyzeSuffixBaisc, SymbolKind.NamedType);
        }

        private void SymbolAnalyzeSuffixBaisc(SymbolAnalysisContext context)
        {
            var name = context.Symbol.Name;
            if (ReservedNames.Contains(name))
                return;

            var match = SuffixRegex.Match(name);
            if (match.Success)
            {
                var typeSymbol = (INamedTypeSymbol)context.Symbol;
                if (!IsClass(typeSymbol))
                    return;

                if (!HasModelsNamespace(typeSymbol))
                    return;

                var suffix = match.Groups["Suffix"].Value;
                var diagnostic = Diagnostic.Create(Rule, context.Symbol.Locations[0],
                    new Dictionary<string, string> { { "SuggestedName", name.Substring(0, name.Length - suffix.Length) } }.ToImmutableDictionary(), name, suffix);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
