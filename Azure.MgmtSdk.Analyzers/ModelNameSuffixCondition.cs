using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System;
using System.ComponentModel;

namespace Azure.MgmtSdk.Analyzers
{
    /// <summary>
    /// Analyzer to check type name suffixes. There are some suffixed we don't recommend to use, like `Result`, `Response`...
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ModelNameSuffixCondition : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AZM0011";

        private static readonly string Title = "Imroper model name suffix";
        private static readonly string MessageFormat = "Model name '{0}' ends with '{1}'";
        private static readonly string Description = "Suffix is not recommended. Consider to remove or modify.";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title,
            MessageFormat, DiagnosticCategory.Naming, DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: Description);

        private bool underModelsNamespace; // Judge if a "xxx.Models" namespace which define a Model.
        // ConditionOne: Avoid using Definition as model suffix unless it's the name of a Resource
        private static readonly Regex SuffixRegexConditionOne = new Regex(".+(?<Suffix>(Definition))$");
        // ConditionTwo: Avoid using Data as model suffix unless the model derives from ResourceData/TrackedResourceData
        private static readonly Regex SuffixRegexConditionTwo = new Regex(".+(?<Suffix>(Data))$");
        // ConditionThree: Avoid using Operation as model suffix unless the model derives from Operation<T>
        private static readonly Regex SuffixRegexConditionThree = new Regex(".+(?<Suffix>(Operation))$");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(SyntaxAnalyzeSuffix, SyntaxKind.ClassDeclaration);
            context.RegisterSymbolAction(AnalyzeSuffixConditionOne, SymbolKind.NamedType);
            context.RegisterSymbolAction(AnalyzeSuffixConditionTwo, SymbolKind.NamedType);
        }
        private void SyntaxAnalyzeSuffix(SyntaxNodeAnalysisContext context)
        {
            var classNode = context.Node;
            var model = context.SemanticModel;
            var classSymbol = model.GetDeclaredSymbol(classNode);
            string fullNamespace = classSymbol.ContainingNamespace.Name;
            if (fullNamespace.Contains("Models"))
            {
                underModelsNamespace = true;
            }
            else
            {
                underModelsNamespace = false;
            }
        }

        public static bool DerivesFromOrImplementsAnyConstructionOf(INamedTypeSymbol type, INamedTypeSymbol parentType)
        {
            if (!parentType.IsDefinition)
            {
                throw new ArgumentException($"The type {nameof(parentType)} is not a definition; it is a constructed type", nameof(parentType));
            }

            for (INamedTypeSymbol baseType = type.OriginalDefinition;
                baseType != null;
                baseType = baseType.BaseType?.OriginalDefinition)
            {
                if (baseType.Equals(parentType))
                {
                    return true;
                }
            }

            return false;
        }

        private void AnalyzeSuffixConditionOne(SymbolAnalysisContext context)
        {
            if (!underModelsNamespace)
                return;
            var name = context.Symbol.Name;
            Compilation compilation = context.Compilation;
            INamedTypeSymbol typeSymbol = compilation.GetTypeByMetadataName(name);
            INamedTypeSymbol parentTypeSymbol = compilation.GetTypeByMetadataName("ArmResource");

            if (DerivesFromOrImplementsAnyConstructionOf(typeSymbol, parentTypeSymbol))
                return;

            var match = SuffixRegexConditionOne.Match(name);
            if (match.Success)
            {
                var suffix = match.Groups["Suffix"].Value;
                var diagnostic = Diagnostic.Create(Rule, context.Symbol.Locations[0],
                    new Dictionary<string, string> { { "SuggestedName", name.Substring(0, name.Length - suffix.Length) } }.ToImmutableDictionary(), name, suffix);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeSuffixConditionTwo(SymbolAnalysisContext context)
        {
            if (!underModelsNamespace)
                return;
            var name = context.Symbol.Name;
            //var typeSymbol = (INamedTypeSymbol)context.Symbol;
            //if (typeSymbol.ToString() == "ResourceData" || typeSymbol.ToString() == "TrackedResourceData")
            //    return;
            //Compilation compilation = context.Compilation;
            //INamedTypeSymbol typeSymbol = compilation.GetTypeByMetadataName(name);
            //Console.WriteLine("typeSymbol: ", typeSymbol);
            //if (typeSymbol.ToString() == "ResourceData" || typeSymbol.ToString() == "TrackedResourceData")
            //    return;

            //while (typeSymbol.BaseType != null)
            //{
            //    typeSymbol = typeSymbol.BaseType;
            //    if (typeSymbol.ToString() == "ResourceData" || typeSymbol.ToString() == "TrackedResourceData")
            //    {
            //        return;
            //    }
            //}
            Compilation compilation = context.Compilation;
            INamedTypeSymbol typeSymbol = compilation.GetTypeByMetadataName(name);
            INamedTypeSymbol parentTypeSymbolOne = compilation.GetTypeByMetadataName("ResourceData");
            INamedTypeSymbol parentTypeSymbolTwo = compilation.GetTypeByMetadataName("TrackedResourceData");

            if (DerivesFromOrImplementsAnyConstructionOf(typeSymbol, parentTypeSymbolOne) || DerivesFromOrImplementsAnyConstructionOf(typeSymbol, parentTypeSymbolTwo))
                return;

            var match = SuffixRegexConditionTwo.Match(name);
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
