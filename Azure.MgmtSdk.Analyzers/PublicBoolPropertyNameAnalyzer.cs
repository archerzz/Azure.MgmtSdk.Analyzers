using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Azure.MgmtSdk.Analyzers
{
    /// <summary>
    /// Analyzer to check type name suffixes. This file contains the base class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PublicBoolPropertyNameAnalyzer : DiagnosticAnalyzer
    {
        protected static readonly string Title = "Improper public bool property";
        protected static readonly string MessageFormat = "Boolean property name '{0}' should have prefix 'Is', 'Has', 'Can', or 'Enable'";
        protected static readonly string Description = "PropertyName is not recommended. Consider to add a verb as prefix.";

        public const string DiagnosticId = "AZM0020";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title,
            MessageFormat, DiagnosticCategory.Naming, DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: Description);

        // Model suffix forbidden
        private static readonly Regex prefixRegex = new Regex("(?<Prefix>(Is)|(Has)|(Can)|(Enable)).+$");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(SyntaxAnalyzePublicBoolPropertyName, SyntaxKind.PropertyDeclaration);
        }

        private void SyntaxAnalyzePublicBoolPropertyName(SyntaxNodeAnalysisContext context)
        {
            PropertyDeclarationSyntax node = (PropertyDeclarationSyntax)context.Node;
            if (AnalyzerUtils.IsNotSdkCode(node, context.SemanticModel))
            {
                return;
            }

            var variableName = node.Identifier.ToString();
            var variableType = node.Type;

            if (!variableType.ToString().Contains("bool")) // include 'bool' and 'bool?'
                return;

            var match = prefixRegex.Match(variableName);

            if (!match.Success)
            {
                var diagnostic = Diagnostic.Create(Rule, context.ContainingSymbol.Locations[0],
                    new Dictionary<string, string> { { "SuggestedName", variableName.Substring(0, variableName.Length) } }.ToImmutableDictionary(), variableName);
                context.ReportDiagnostic(diagnostic);
            }

        }

    }
}
