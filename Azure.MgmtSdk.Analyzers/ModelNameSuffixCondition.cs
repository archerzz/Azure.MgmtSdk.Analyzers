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
    /// Analyzer to check type name suffixes. There are some suffixed we don't recommend to use, like `Result`, `Response`...
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ModelNameSuffixDefinitionAnalyzer : ModelNameSuffixAnalyzer
    {
        public const string DiagnosticIdDefinition = DiagnosticIdBase + "C1";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIdDefinition, Title,
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
    public class ModelNameSuffixDataAnalyzer : ModelNameSuffixAnalyzer
    {
        public const string DiagnosticIdData = DiagnosticIdBase + "C2";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIdData, Title,
            MessageFormat, DiagnosticCategory.Naming, DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: Description);

        //ConditionData: Avoid using Data as model suffix unless the model derives from ResourceData/TrackedResourceData
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

    public class ModelNameSuffixOperationAnalyzer : ModelNameSuffixAnalyzer
    {
        public const string DiagnosticIdOperation = DiagnosticIdBase + "C3";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIdOperation, Title,
            MessageFormat, DiagnosticCategory.Naming, DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: Description);


        //ConditionOperation: Avoid using Operation as model suffix unless the model derives from Operation<T>
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

}
