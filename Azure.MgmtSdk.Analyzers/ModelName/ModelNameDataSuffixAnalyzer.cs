using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Azure.MgmtSdk.Analyzers.ModelName
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ModelNameSuffixDataAnalyzer : ModelNameSuffixAnalyzerBase
    {
        public const string DiagnosticId = "AZM0012";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title,
            GeneralRenamingMessageFormat, DiagnosticCategory.Naming, DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: Description);

        // ConditionData: Avoid using Data as model suffix unless the model derives from ResourceData/TrackedResourceData
        private static readonly Regex DataSuffixRegex = new Regex(".+(?<Suffix>(Data))$");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        protected override bool ShouldSkip(INamedTypeSymbol symbol) => IsTypeOf(symbol, "Azure.ResourceManager.Models", "ResourceData") ||
            IsTypeOf(symbol, "Azure.ResourceManager.Models", "TrackedResourceData") ||
            AnalyzerUtils.IsNotSdkCode(symbol);

        protected override Regex MatchRegex => DataSuffixRegex;
        protected override Diagnostic GetDiagnostic(INamedTypeSymbol typeSymbol, string suffix, SymbolAnalysisContext context)
        {
            var name = typeSymbol.Name;
            return Diagnostic.Create(Rule, context.Symbol.Locations[0],
                new Dictionary<string, string> { { "SuggestedName", name.Substring(0, name.Length - suffix.Length) } }.ToImmutableDictionary(), name, suffix);
        }
    }
}
