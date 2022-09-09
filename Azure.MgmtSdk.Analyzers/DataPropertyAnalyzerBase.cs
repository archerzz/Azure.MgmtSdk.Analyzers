using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace Azure.MgmtSdk.Analyzers
{
    /// <summary>
    /// Analyzer to check type name suffixes. This file contains the base class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DataPropertyAnalyzerBase : DiagnosticAnalyzer
    {
        protected static readonly string Title = "Potential improper data type of propert";
        protected static readonly string MessageFormat = "Property {0} looks like an '{1}'.";
        protected static readonly string Description = "Check the real return value and consider changing the type to '{1}'.";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray<DiagnosticDescriptor>.Empty;

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
        }


        protected void MatchAndDiagnostic(Regex suffixRegex, string variableName, string variableType, List<string> targetName, List<string> targetType, DiagnosticDescriptor Rule, SyntaxNodeAnalysisContext context, List<string> checkType = null)
        {
            var match = suffixRegex.Match(variableName);
            
            if (match.Success || targetName.Exists(item => item == variableName))
            {
                if (targetType.Exists(item => item == variableType))
                    return;

                if (checkType != null && !checkType.Exists(item => item == variableType))
                    return;

                var diagnostic = Diagnostic.Create(Rule, context.ContainingSymbol.Locations[0], variableName, variableType);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
