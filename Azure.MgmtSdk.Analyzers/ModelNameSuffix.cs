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
    public class ModelNameSuffix : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AZM0010";

        private static readonly string Title = "Imroper model name suffix";
        private static readonly string MessageFormat = "Model name '{0}' ends with '{1}'";
        private static readonly string Description = "Suffix is not recommended. Consider to remove or modify.";

        private static readonly HashSet<string> ReservedNames = new HashSet<string> { "ErrorResponse" };

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title,
            MessageFormat, DiagnosticCategory.Naming, DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: Description);

        public bool underModelsNamespace; // Judge if a "xxx.Models" namespace which define a Model.
        // Model suffix forbidden
        private static readonly Regex SuffixRegex = new Regex(".+(?<Suffix>(Results?)|(Requests?)|(Responses?)|(Parameters?)|(Options?)|(Collection)|(Resource))$");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(SymbolAnalyzeSuffix, SymbolKind.NamedType);
        }

        private void SymbolAnalyzeSuffix(SymbolAnalysisContext context)
        {
            var name = context.Symbol.Name; 
            if (ReservedNames.Contains(name))
                return;

            var typeSymbol = (INamedTypeSymbol)context.Symbol;
            if (typeSymbol.TypeKind != TypeKind.Class) // We just check classes.
                return;

            //var namespaceSymbolA = typeSymbol.ContainingNamespace;
            //Console.WriteLine("{0}{1}{2}", "Symbol namespaceSymbol: ", namespaceSymbolA, namespaceSymbolA.GetType());
            bool hasNamespaceModels = false;

            //if (namespaceSymbolA.ToString().EndsWith("Models"))
            //    hasNamespaceModels = true;

            for (var namespaceSymbol = typeSymbol.ContainingNamespace; namespaceSymbol != null; namespaceSymbol = namespaceSymbol.ContainingNamespace)
            {
                //Console.WriteLine("{0}{1}", "Symbol namespaceSymbol: ", namespaceSymbol.GetType());
                var fullNamespace = namespaceSymbol.Name;
                if (fullNamespace.Contains("Models"))
                {
                    hasNamespaceModels = true;
                    break;
                }
            }

            // Console.WriteLine("{0}{1}", "Symbol hasNamespaceModels: ", hasNamespaceModels);
            if (!hasNamespaceModels)
                return;

            //Console.WriteLine("{0}{1}", "final name: ", name);
            var match = SuffixRegex.Match(name);
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
