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
    /// Analyzer to check resource type 'ResourceIdentifier'.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DataPropertyResourceIdentifierAnalyzer : DiagnosticAnalyzer
    {
        protected static readonly string Title = "Improper public data property";
        protected static readonly string MessageFormat = "The data type of a property name '{0}' is '{1}'.";
        protected static readonly string Description = "Consider to change it to ResourceIdentifier.";

        public const string DiagnosticIdDataPropertyResourceIdentifierName = "AZM0041";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIdDataPropertyResourceIdentifierName, Title,
            MessageFormat, DiagnosticCategory.Naming, DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: Description);

        private static readonly Regex suffixRegex = new Regex(".+(?<Suffix>(Id))$");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(SyntaxAnalyzeDataPropertyResourceIdentifierVariableName, SyntaxKind.VariableDeclaration);
            context.RegisterSyntaxNodeAction(SyntaxAnalyzeDataPropertyResourceIdentifierPropertyName, SyntaxKind.PropertyDeclaration);
        }

        private void SyntaxAnalyzeDataPropertyResourceIdentifierVariableName(SyntaxNodeAnalysisContext context)
        {
            VariableDeclarationSyntax node = (VariableDeclarationSyntax)context.Node;
            var variableName = node.Variables.ToString();
            var variableType = node.Type;

            var match = suffixRegex.Match(variableName);

            if (match.Success)
            {
                if (variableType.ToString().Contains("ResourceIdentifier")) // ResourceIdentifier and Azure.Core.ResourceIdentifier
                    return;

                var diagnostic = Diagnostic.Create(Rule, context.ContainingSymbol.Locations[0],
                    new Dictionary<string, string> { { "SuggestedName", variableName.Substring(0, variableName.Length) } }.ToImmutableDictionary(), variableName, variableType.ToString());
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void SyntaxAnalyzeDataPropertyResourceIdentifierPropertyName(SyntaxNodeAnalysisContext context)
        {
            PropertyDeclarationSyntax node = (PropertyDeclarationSyntax)context.Node;
            var variableName = node.Identifier.ToString();
            var variableType = node.Type;

            var match = suffixRegex.Match(variableName);

            if (match.Success)
            {
                if (variableType.ToString().Contains("ResourceIdentifier")) // ResourceIdentifier and Azure.Core.ResourceIdentifier
                    return;

                var diagnostic = Diagnostic.Create(Rule, context.ContainingSymbol.Locations[0],
                    new Dictionary<string, string> { { "SuggestedName", variableName.Substring(0, variableName.Length) } }.ToImmutableDictionary(), variableName, variableType.ToString());
                context.ReportDiagnostic(diagnostic);
            }
        }

    }
}
