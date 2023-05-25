using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;

namespace Azure.MgmtSdk.Analyzers.ModelName
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ModelNameResourceSuffixAnalyzer : ModelNameSuffixAnalyzerBase
    {
        public const string DiagnosticId = "AZM0014";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title,
            GeneralRenamingMessageFormat, DiagnosticCategory.Naming, DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: Description);

        // ConditionResource: Avoid using Resource as model suffix unless it's the name of GenericResource, PrivateLinkServiceResource, etc.
        private static readonly Regex ResourceSuffixRegex = new Regex(".+(?<Suffix>(Resource))$");
        private static readonly HashSet<string> ReservedResourceNames = new HashSet<string> { "GenericResource", "PrivateLinkServiceResource" };

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        protected override bool ShouldSkip(INamedTypeSymbol symbol) => ReservedResourceNames.Contains(symbol.Name);
        protected override Regex MatchRegex => ResourceSuffixRegex;
        protected override Diagnostic GetDiagnostic(INamedTypeSymbol typeSymbol, string suffix, SymbolAnalysisContext context)
        {
            var name = typeSymbol.Name;
            return Diagnostic.Create(Rule, context.Symbol.Locations[0],
                new Dictionary<string, string> { { "SuggestedName", name.Substring(0, name.Length - suffix.Length) } }.ToImmutableDictionary(), name, suffix);
        }
    }
}
