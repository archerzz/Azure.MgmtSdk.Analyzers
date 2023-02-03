using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System;
using System.ComponentModel;

namespace Azure.MgmtSdk.Analyzers.ModelName
{
    /// <summary>
    /// Analyzer to check type name suffixes. This file contains some methods that deal with different conditions.
    /// </summary>
    public class ModelNameSuffixGeneralAnalyzer : ModelNameSuffixAnalyzerBase
    {
        public const string DiagnosticId = "AZM0010";

        private static readonly string MessageFormat = "Model name '{0}' ends with '{1}'. Suggest to rename it to {2} or any other appropriate name.";

        private static readonly HashSet<string> ReservedNames = new HashSet<string> { "ErrorResponse" };

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title,
            MessageFormat, DiagnosticCategory.Naming, DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: Description);

        // Problematic model suffix
        private static readonly Regex SuffixRegex = new Regex(".+(?<Suffix>(Requests?)|(Responses?)|(Parameters?)|(Options?)|(Collection))$");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        protected override bool ShouldSkip(INamedTypeSymbol symbol) => ReservedNames.Contains(symbol.Name);
        protected override Regex MatchRegex => SuffixRegex;
        protected override Diagnostic GetDiagnostic(INamedTypeSymbol typeSymbol, string suffix, SymbolAnalysisContext context)
        {
            var name = typeSymbol.Name;
            var suggestedName = GetSuggestedName(name, suffix);
            return Diagnostic.Create(Rule, context.Symbol.Locations[0],
                new Dictionary<string, string> { { "SuggestedName", suggestedName } }.ToImmutableDictionary(), name, suffix, suggestedName);
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
                "Collection" => $"'{nameWithoutSuffix}Group' or '{nameWithoutSuffix}List'",
                _ => nameWithoutSuffix,
            };
        }
    }



}
