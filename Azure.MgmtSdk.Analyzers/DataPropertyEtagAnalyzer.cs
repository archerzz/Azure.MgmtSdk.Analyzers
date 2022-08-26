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
    /// Analyzer to check resource type 'ResouceType'.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DataPropertyEtagAnalyzer : DiagnosticAnalyzer
    {
        protected static readonly string Title = "Improper public data property";
        protected static readonly string MessageFormat = "The data type of a property name '{0}' is '{1}'.";
        protected static readonly string Description = "Consider to change it to Etag.";

        public const string DiagnosticIdDataPropertyETagName = "AZM0043";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIdDataPropertyETagName, Title,
            MessageFormat, DiagnosticCategory.Naming, DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: Description);

        private static readonly Regex suffixRegex = new Regex(".+(?<Suffix>([Ee][Tt]ag))$");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(SyntaxAnalyzeDataPropertyEtagVariableName, SyntaxKind.VariableDeclaration);
            context.RegisterSyntaxNodeAction(SyntaxAnalyzeDataPropertyEtagPropertyName, SyntaxKind.PropertyDeclaration);
        }

        private void SyntaxAnalyzeDataPropertyEtagVariableName(SyntaxNodeAnalysisContext context)
        {
            VariableDeclarationSyntax node = (VariableDeclarationSyntax)context.Node;
            var variableName = node.Variables.ToString();
            var variableType = node.Type;

            var match = suffixRegex.Match(variableName);

            Console.WriteLine(variableType.ToString());

            if (match.Success || variableName == "Etag")
            {
                if (variableType.ToString().Contains("ETag") || variableType.ToString().Contains("etag"))
                    return;

                var diagnostic = Diagnostic.Create(Rule, context.ContainingSymbol.Locations[0],
                    new Dictionary<string, string> { { "SuggestedName", variableName.Substring(0, variableName.Length) } }.ToImmutableDictionary(), variableName, variableType.ToString());
                context.ReportDiagnostic(diagnostic);
            }

        }

        private void SyntaxAnalyzeDataPropertyEtagPropertyName(SyntaxNodeAnalysisContext context)
        {
            PropertyDeclarationSyntax node = (PropertyDeclarationSyntax)context.Node;
            var variableName = node.Identifier.ToString();
            var variableType = node.Type;

            var match = suffixRegex.Match(variableName);

            if (match.Success || variableName == "Etag")
            {
                if (variableType.ToString().Contains("ETag") || variableType.ToString().Contains("etag"))
                    return;

                var diagnostic = Diagnostic.Create(Rule, context.ContainingSymbol.Locations[0],
                    new Dictionary<string, string> { { "SuggestedName", variableName.Substring(0, variableName.Length) } }.ToImmutableDictionary(), variableName, variableType.ToString());
                context.ReportDiagnostic(diagnostic);
            }
        }

    }
}
