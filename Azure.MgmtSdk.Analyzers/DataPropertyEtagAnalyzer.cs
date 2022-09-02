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
    public class DataPropertyEtagAnalyzer : DataPropertyAnalyzerBase
    {
        public const string DiagnosticId = "AZM0043";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title,
            MessageFormat, DiagnosticCategory.Naming, DiagnosticSeverity.Info, isEnabledByDefault: true,
            description: Description);

        private static readonly Regex suffixRegex = new Regex("");
        protected static List<string> targetName = new List<string> { "Etag", "ETag" };
        protected static List<string> targetType = new List<string> { "ETag", "ETag?", "Azure.Core.ETag", "Azure.Core.ETag?" };

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
            var variableType = node.Type.ToString();
            MatchAndDiagnostic(suffixRegex, variableName, variableType, targetName, targetType, Rule, context);

            //var match = suffixRegex.Match(variableName);

            //Console.WriteLine(variableType.ToString());

            //if (match.Success || variableName == "Etag")
            //{
            //    if (variableType.ToString().Contains("ETag") || variableType.ToString().Contains("etag"))
            //        return;

            //    var diagnostic = Diagnostic.Create(Rule, context.ContainingSymbol.Locations[0],
            //        new Dictionary<string, string> { { "SuggestedName", variableName.Substring(0, variableName.Length) } }.ToImmutableDictionary(), variableName, variableType.ToString());
            //    context.ReportDiagnostic(diagnostic);
            //}

        }

        private void SyntaxAnalyzeDataPropertyEtagPropertyName(SyntaxNodeAnalysisContext context)
        {
            PropertyDeclarationSyntax node = (PropertyDeclarationSyntax)context.Node;
            var variableName = node.Identifier.ToString();
            var variableType = node.Type.ToString();
            MatchAndDiagnostic(suffixRegex, variableName, variableType, targetName, targetType, Rule, context);

            //var match = suffixRegex.Match(variableName);

            //if (match.Success || variableName == "Etag")
            //{
            //    if (variableType.ToString().Contains("ETag") || variableType.ToString().Contains("etag"))
            //        return;

            //    var diagnostic = Diagnostic.Create(Rule, context.ContainingSymbol.Locations[0], variableName);
            //        //new Dictionary<string, string> { { "SuggestedName", variableName.Substring(0, variableName.Length) } }.ToImmutableDictionary(), variableName);
            //    context.ReportDiagnostic(diagnostic);
            //}
        }

    }
}
