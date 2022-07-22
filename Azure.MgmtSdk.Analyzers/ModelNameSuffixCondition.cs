using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System;

namespace Azure.MgmtSdk.Analyzers
{
    /// <summary>
    /// Analyzer to check type name suffixes. There are some suffixed we don't recommend to use, like `Result`, `Response`...
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ModelNameSuffixCondition : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AZM0011";

        private static readonly string Title = "Imroper model name suffix";
        private static readonly string MessageFormat = "Model name '{0}' ends with '{1}'";
        private static readonly string Description = "Suffix is not recommended. Consider to remove or modify.";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title,
            MessageFormat, DiagnosticCategory.Naming, DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: Description);

        // ConditionOne: Avoid using Definition as model suffix unless it's the name of a Resource
        private static readonly Regex SuffixRegexConditionOne = new Regex(".+(?<Suffix>(Definition))$");
        // ConditionTwo: Avoid using Data as model suffix unless the model derives from ResourceData/TrackedResourceData
        private static readonly Regex SuffixRegexConditionTwo = new Regex(".+(?<Suffix>(Data))$");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeSuffixConditionOne, SymbolKind.NamedType);
            context.RegisterSymbolAction(AnalyzeSuffixConditionTwo, SymbolKind.NamedType);
        }

        private void AnalyzeSuffixConditionOne(SymbolAnalysisContext context)
        {
            var name = context.Symbol.Name;
            var typeSymbol = (INamedTypeSymbol)context.Symbol;
            if (typeSymbol.ToString() == "ArmResource")
            {
                return;
            }

            var match = SuffixRegexConditionOne.Match(name);
            if (match.Success)
            {
                var suffix = match.Groups["Suffix"].Value;
                var diagnostic = Diagnostic.Create(Rule, context.Symbol.Locations[0],
                    new Dictionary<string, string> { { "SuggestedName", name.Substring(0, name.Length - suffix.Length) } }.ToImmutableDictionary(), name, suffix);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeSuffixConditionTwo(SymbolAnalysisContext context)
        {
            var name = context.Symbol.Name;
            var typeSymbol = (INamedTypeSymbol)context.Symbol;
            if (typeSymbol.ToString() == "ResourceData" || typeSymbol.ToString() == "TrackedResourceData")
            {
                return;
            }
            while (typeSymbol.BaseType != null)
            {
                typeSymbol = typeSymbol.BaseType;
                if (typeSymbol.ToString() == "ResourceData" || typeSymbol.ToString() == "TrackedResourceData")
                {
                    return;
                }
            }

            var match = SuffixRegexConditionTwo.Match(name);
            if (match.Success)
            {
                var suffix = match.Groups["Suffix"].Value;
                var diagnostic = Diagnostic.Create(Rule, context.Symbol.Locations[0],
                    new Dictionary<string, string> { { "SuggestedName", name.Substring(0, name.Length - suffix.Length) } }.ToImmutableDictionary(), name, suffix);
                context.ReportDiagnostic(diagnostic);
            }
        }

    }
}
