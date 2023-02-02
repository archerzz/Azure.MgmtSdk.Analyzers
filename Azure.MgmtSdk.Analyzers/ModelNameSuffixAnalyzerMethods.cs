using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System;
using System.ComponentModel;

namespace Azure.MgmtSdk.Analyzers
{
    /// <summary>
    /// Analyzer to check type name suffixes. This file contains some methods that deal with different conditions.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]

    public class ModelNameSuffixGeneralAnalyzer : ModelNameSuffixAnalyzerBase
    {
        public const string DiagnosticId = "AZM0010";

        private static readonly HashSet<string> ReservedNames = new HashSet<string> { "ErrorResponse" };

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title,
            MessageFormat, DiagnosticCategory.Naming, DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: Description);

        // Problematic model suffix
        private static readonly Regex SuffixRegex = new Regex(".+(?<Suffix>(Requests?)|(Responses?)|(Parameters?)|(Options?)|(Collection))$");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(SymbolAnalyzeSuffixBaisc, SymbolKind.NamedType);
        }

        private void SymbolAnalyzeSuffixBaisc(SymbolAnalysisContext context)
        {
            var typeSymbol = (INamedTypeSymbol)context.Symbol;
            if (!IsClass(typeSymbol))
                return;

            var name = typeSymbol.Name;
            if (ReservedNames.Contains(name))
                return;

            var match = SuffixRegex.Match(name);
            if (match.Success)
            {
                if (!HasModelsNamespace(typeSymbol))
                    return;

                var suffix = match.Groups["Suffix"].Value;
                var suggestedName = GetSuggestedName(name, suffix);
                var diagnostic = Diagnostic.Create(Rule, context.Symbol.Locations[0],
                    new Dictionary<string, string> { { "SuggestedName", suggestedName } }.ToImmutableDictionary(), name, suffix, suggestedName);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private string GetSuggestedName(string originalName, string suffix)
        {
            var nameWithoutSuffix = originalName.Substring(0, originalName.Length - suffix.Length);
            return suffix switch
            {
                "Request" or "Requests" => $"'{nameWithoutSuffix}Content'",
                "Parameter" or "Parameters" => $"'{nameWithoutSuffix}Content' or '{nameWithoutSuffix}Patch'",
                "Option" or "Options" => $"'{nameWithoutSuffix}Config'",
                "Response" => $"'{nameWithoutSuffix}Result'",
                "Responses" => $"'{nameWithoutSuffix}Results'",
                "Collection" => $"'{nameWithoutSuffix}Group' or '{nameWithoutSuffix}List'"
            };
        }
    }

    public class ModelNameSuffixDefinitionAnalyzer : ModelNameSuffixAnalyzerBase
    {
        public const string DiagnosticId = "AZM0011";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title,
            MessageFormat, DiagnosticCategory.Naming, DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: Description);

        // ConditionDefinition: Avoid using Definition as model suffix unless it's the name of a Resource
        private static readonly Regex SuffixRegexConditionDefinition = new Regex(".+(?<Suffix>(Definition?))$");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeSuffixConditionDefinition, SymbolKind.NamedType);
        }

        private void AnalyzeSuffixConditionDefinition(SymbolAnalysisContext context)
        {
            var name = context.Symbol.Name;
            var match = SuffixRegexConditionDefinition.Match(name);

            if (match.Success)
            {
                var typeSymbol = (INamedTypeSymbol)context.Symbol;
                if (!IsClass(typeSymbol))
                    return;

                if (!HasModelsNamespace(typeSymbol))
                    return;

                if (ImplementsInterfaceOrBaseClass(typeSymbol, "ArmResource"))
                    return;

                var suffix = match.Groups["Suffix"].Value;
                var diagnostic = Diagnostic.Create(Rule, context.Symbol.Locations[0],
                    new Dictionary<string, string> { { "SuggestedName", name.Substring(0, name.Length - suffix.Length) } }.ToImmutableDictionary(), name, suffix);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    public class ModelNameSuffixDataAnalyzer : ModelNameSuffixAnalyzerBase
    {
        public const string DiagnosticId = "AZM0012";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title,
            MessageFormat, DiagnosticCategory.Naming, DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: Description);

        // ConditionData: Avoid using Data as model suffix unless the model derives from ResourceData/TrackedResourceData
        private static readonly Regex SuffixRegexConditionData = new Regex(".+(?<Suffix>(Data))$");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeSuffixConditionData, SymbolKind.NamedType);
        }

        private void AnalyzeSuffixConditionData(SymbolAnalysisContext context)
        {
            var name = context.Symbol.Name;
            var match = SuffixRegexConditionData.Match(name);

            if (match.Success)
            {
                var typeSymbol = (INamedTypeSymbol)context.Symbol;
                if (!IsClass(typeSymbol))
                    return;

                if (!HasModelsNamespace(typeSymbol))
                    return;

                if (ImplementsInterfaceOrBaseClass(typeSymbol, "ResourceData") || ImplementsInterfaceOrBaseClass(typeSymbol, "TrackedResourceData"))
                    return;

                var suffix = match.Groups["Suffix"].Value;
                var diagnostic = Diagnostic.Create(Rule, context.Symbol.Locations[0],
                    new Dictionary<string, string> { { "SuggestedName", name.Substring(0, name.Length - suffix.Length) } }.ToImmutableDictionary(), name, suffix);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    public class ModelNameSuffixOperationAnalyzer : ModelNameSuffixAnalyzerBase
    {
        public const string DiagnosticId = "AZM0013";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title,
            MessageFormat, DiagnosticCategory.Naming, DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: Description);


        // ConditionOperation: Avoid using Operation as model suffix unless the model derives from Operation<T>
        private static readonly Regex SuffixRegexConditionOperation = new Regex(".+(?<Suffix>(Operation))$");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeSuffixConditionOperation, SymbolKind.NamedType);
        }

        private void AnalyzeSuffixConditionOperation(SymbolAnalysisContext context)
        {
            var name = context.Symbol.Name;
            var match = SuffixRegexConditionOperation.Match(name);

            if (match.Success)
            {
                var typeSymbol = (INamedTypeSymbol)context.Symbol;
                if (!IsClass(typeSymbol))
                    return;

                if (!HasModelsNamespace(typeSymbol))
                    return;

                if (typeSymbol.ToString().EndsWith("<T>"))
                    return;

                if (ImplementsInterfaceOrBaseClass(typeSymbol, "Operation<T>"))
                    return;

                var suffix = match.Groups["Suffix"].Value;
                var diagnostic = Diagnostic.Create(Rule, context.Symbol.Locations[0],
                    new Dictionary<string, string> { { "SuggestedName", name.Substring(0, name.Length - suffix.Length) } }.ToImmutableDictionary(), name, suffix);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    public class ModelNameSuffixResourceAnalyzer : ModelNameSuffixAnalyzerBase
    {
        public const string DiagnosticId = "AZM0014";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title,
            MessageFormat, DiagnosticCategory.Naming, DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: Description);

        // ConditionResource: Avoid using Resource as model suffix unless it's the name of GenericResource, PrivateLinkServiceResource, etc.
        private static readonly Regex SuffixRegexConditionResource = new Regex(".+(?<Suffix>(Resource))$");
        private static readonly HashSet<string> ReservedResourceNames = new HashSet<string> { "GenericResource", "PrivateLinkServiceResource" };

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeSuffixConditionResource, SymbolKind.NamedType);
        }

        private void AnalyzeSuffixConditionResource(SymbolAnalysisContext context)
        {
            var name = context.Symbol.Name;
            var match = SuffixRegexConditionResource.Match(name);

            if (ReservedResourceNames.Contains(name))
                return;

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
