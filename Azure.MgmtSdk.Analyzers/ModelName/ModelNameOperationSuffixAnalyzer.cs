using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;

namespace Azure.MgmtSdk.Analyzers.ModelName
{
    public class ModelNameOperationSuffixAnalyzer : ModelNameSuffixAnalyzerBase
    {
        public const string DiagnosticId = "AZM0013";

        private static readonly string MessageFormat = "Model name '{0}' ends with '{1}'. Suggest to rename it to '{2}' or '{3}', if an appropriate name could not be found.";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title,
            MessageFormat, DiagnosticCategory.Naming, DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: Description);


        // ConditionOperation: Avoid using Operation as model suffix unless the model derives from Operation<T>
        private static readonly Regex OperationSuffixRegex = new Regex(".+(?<Suffix>(Operation))$");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        protected override bool ShouldSkip(INamedTypeSymbol symbol) => IsTypeOf(symbol, "Azure", "Operation");
        protected override Regex MatchRegex => OperationSuffixRegex;
        protected override Diagnostic GetDiagnostic(INamedTypeSymbol typeSymbol, string suffix, SymbolAnalysisContext context)
        {
            var name = typeSymbol.Name;
            var nameWithoutSuffix = name.Substring(0, name.Length - suffix.Length);
            return Diagnostic.Create(Rule, context.Symbol.Locations[0],
                name, suffix, $"{nameWithoutSuffix}Data", $"{nameWithoutSuffix}Info");
        }
    }
}
