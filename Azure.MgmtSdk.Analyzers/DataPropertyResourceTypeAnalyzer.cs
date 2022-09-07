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
    public class DataPropertyResourceTypeAnalyzer : DataPropertyAnalyzerBase
    {
        public const string DiagnosticId = "AZM0042";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title,
            MessageFormat, DiagnosticCategory.Naming, DiagnosticSeverity.Info, isEnabledByDefault: true,
            description: Description);

        private static readonly Regex suffixRegex = new Regex("");
        protected static List<string> targetName = new List<string> { "ResourceType" };
        protected static List<string> targetType = new List<string> { "ResourceType", "ResourceType?", "Azure.Core.ResourceType", "Azure.Core.ResourceType?" };

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
            var variableType = node.Type.ToString();
            MatchAndDiagnostic(suffixRegex, variableName, variableType, targetName, targetType, Rule, context);
        }

        private void SyntaxAnalyzeDataPropertyResourceTypePropertyName(SyntaxNodeAnalysisContext context)
        {
            PropertyDeclarationSyntax node = (PropertyDeclarationSyntax)context.Node;
            var variableName = node.Identifier.ToString();
            var variableType = node.Type.ToString();
            MatchAndDiagnostic(suffixRegex, variableName, variableType, targetName, targetType, Rule, context);
        }

    }
}
