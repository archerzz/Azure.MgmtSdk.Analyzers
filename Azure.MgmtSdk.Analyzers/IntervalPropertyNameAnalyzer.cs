using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;

namespace Azure.MgmtSdk.Analyzers
{
    /// <summary>
    /// Analyzer to check interval property name.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IntervalPropertyNameAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AZM0030";

        protected static readonly string Title = "Improper interval property name";
        protected static readonly string MessageFormat = "Property name '{0}' should end with units";
        protected static readonly string Description = "Property is of integer type. Consider to append unit to the name, like \"InSeconds\".";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title,
            MessageFormat, DiagnosticCategory.Naming, DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: Description);

        private static readonly Regex SuffixRegex = new Regex("((Interval)|(Duration))$");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeIntervalProperty, SymbolKind.Property);
        }

        private void AnalyzeIntervalProperty(SymbolAnalysisContext context)
        {
            if (AnalyzerUtils.IsNotSdkCode(context.Symbol))
            {
                return;
            }

            var name = context.Symbol.Name;
            if (SuffixRegex.IsMatch(name))
            {
                var type = ((IPropertySymbol)context.Symbol).Type;

                switch (type.SpecialType)
                {
                    case SpecialType.System_Int16:
                    case SpecialType.System_Int32:
                    case SpecialType.System_Int64:
                    case SpecialType.System_UInt16:
                    case SpecialType.System_UInt32:
                    case SpecialType.System_UInt64:
                        var diagnostic = Diagnostic.Create(Rule, context.Symbol.Locations[0], name);
                        context.ReportDiagnostic(diagnostic);
                        break;
                }
            }
        }
    }
}
