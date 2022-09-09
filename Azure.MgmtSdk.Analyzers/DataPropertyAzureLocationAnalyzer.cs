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
    /// Analyzer to check type 'AzureLocation'.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DataPropertyAzureLocationAnalyzer : DataPropertyAnalyzerBase
    {
        public const string DiagnosticId = "AZM0044";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title,
            MessageFormat, DiagnosticCategory.Naming, DiagnosticSeverity.Info, isEnabledByDefault: true,
            description: Description);

        private static readonly Regex suffixRegex = new Regex("(.+)?(?<Suffix>(Locations?))$");

        protected static List<string> targetName = new List<string> { };
        protected static List<string> targetType = new List<string> { "AzureLocation", "AzureLocation?", "Azure.Core.AzureLocation", "Azure.Core.AzureLocation?" };
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(SyntaxAnalyzeDataPropertyAzureLocationVariableName, SyntaxKind.VariableDeclaration);
            context.RegisterSyntaxNodeAction(SyntaxAnalyzeDataPropertyAzureLocationPropertyName, SyntaxKind.PropertyDeclaration);
        }

        private void SyntaxAnalyzeDataPropertyAzureLocationVariableName(SyntaxNodeAnalysisContext context)
        {
            VariableDeclarationSyntax node = (VariableDeclarationSyntax)context.Node;
            var variableName = node.Variables.ToString();
            var variableType = node.Type.ToString();
            MatchAndDiagnostic(suffixRegex, variableName, variableType, targetName, targetType, Rule, context);
        }

        private void SyntaxAnalyzeDataPropertyAzureLocationPropertyName(SyntaxNodeAnalysisContext context)
        {
            PropertyDeclarationSyntax node = (PropertyDeclarationSyntax)context.Node;
            var variableName = node.Identifier.ToString();
            var variableType = node.Type.ToString();
            MatchAndDiagnostic(suffixRegex, variableName, variableType, targetName, targetType, Rule, context);
        }

    }
}
