using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Azure.MgmtSdk.Analyzers.ModelName
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ModelNameDefinitionSuffixAnalyzer : ModelNameSuffixAnalyzerBase
    {
        public const string DiagnosticId = "AZM0011";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title,
            GeneralRenamingMessageFormat, DiagnosticCategory.Naming, DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: Description);

        // ConditionDefinition: Avoid using Definition as model suffix unless it's the name of a Resource
        private static readonly Regex ConditionSuffixRegex = new Regex(".+(?<Suffix>(Definition?))$");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        protected override bool ShouldSkip(INamedTypeSymbol symbol) => IsTypeOf(symbol, "Azure.ResourceManager", "ArmResource") || 
            AnalyzerUtils.IsNotSdkCode(symbol);
        protected override Regex MatchRegex => ConditionSuffixRegex;
        protected override Diagnostic GetDiagnostic(INamedTypeSymbol typeSymbol, string suffix, SymbolAnalysisContext context)
        {
            var name = typeSymbol.Name;
            return Diagnostic.Create(Rule, context.Symbol.Locations[0],
                new Dictionary<string, string> { { "SuggestedName", name.Substring(0, name.Length - suffix.Length) } }.ToImmutableDictionary(), name, suffix);
        }
    }
}
