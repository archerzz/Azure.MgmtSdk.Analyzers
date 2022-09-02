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
    public class DataPropertyResourceTypeAnalyzer : DiagnosticAnalyzer
    {
        protected static readonly string Title = "Potential improper data type of propert";
        protected static readonly string MessageFormat = "Property {0} looks like a ResourceType.";
        protected static readonly string Description = "Check the real return value and consider changing the type to \"ResourceType\".";

        public const string DiagnosticId = "AZM0042";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title,
            MessageFormat, DiagnosticCategory.Naming, DiagnosticSeverity.Info, isEnabledByDefault: true,
            description: Description);

        private static readonly Regex suffixRegex = new Regex(".+(?<Suffix>(Type))$");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(SyntaxAnalyzeDataPropertyResourceTypeVariableName, SyntaxKind.VariableDeclaration);
            context.RegisterSyntaxNodeAction(SyntaxAnalyzeDataPropertyResourceTypePropertyName, SyntaxKind.PropertyDeclaration);
        }

        private void SyntaxAnalyzeDataPropertyResourceTypeVariableName(SyntaxNodeAnalysisContext context)
        {
            VariableDeclarationSyntax node = (VariableDeclarationSyntax)context.Node;
            var variableName = node.Variables.ToString();
            var variableType = node.Type;

            var match = suffixRegex.Match(variableName);

            if (match.Success || variableName == "ResourceType") // its name is 'ResourceType' or end with 'Type'
            {
                if (variableType.ToString().Contains("ResourceType")) // ResourceType and Azure.Core.ResourceType
                    return;
                var diagnostic = Diagnostic.Create(Rule, context.ContainingSymbol.Locations[0],
                    new Dictionary<string, string> { { "SuggestedName", variableName.Substring(0, variableName.Length) } }.ToImmutableDictionary(), variableName, variableType.ToString());
                context.ReportDiagnostic(diagnostic);
            }

        }

        private void SyntaxAnalyzeDataPropertyResourceTypePropertyName(SyntaxNodeAnalysisContext context)
        {
            PropertyDeclarationSyntax node = (PropertyDeclarationSyntax)context.Node;
            var variableName = node.Identifier.ToString();
            var variableType = node.Type;

            var match = suffixRegex.Match(variableName);

            if (match.Success || variableName == "ResourceType") // its name is 'ResourceType' or end with 'Type'
            {
                if (variableType.ToString().Contains("ResourceType")) // ResourceType and Azure.Core.ResourceType
                    return;

                var diagnostic = Diagnostic.Create(Rule, context.ContainingSymbol.Locations[0],
                    new Dictionary<string, string> { { "SuggestedName", variableName.Substring(0, variableName.Length) } }.ToImmutableDictionary(), variableName);
                context.ReportDiagnostic(diagnostic);
            }
        }

    }
}
