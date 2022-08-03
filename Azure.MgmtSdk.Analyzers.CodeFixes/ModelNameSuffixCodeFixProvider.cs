using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.MgmtSdk.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ModelNameSuffixCodeFixProvider)), Shared]
    public class ModelNameSuffixCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(ModelNameSuffixAnalyzer.DiagnosticIdBase); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Rename type",
                    createChangedDocument: c => InsertAutorestDirectiveInCommentsAsync(context.Document, diagnostic, c),
                    equivalenceKey: nameof(ModelNameSuffixCodeFixProvider)),
                diagnostic);

            return Task.CompletedTask;
        }

        private async Task<Document> InsertAutorestDirectiveInCommentsAsync(Document document, Diagnostic diagnostic, CancellationToken c)
        {
            var root = await document.GetSyntaxRootAsync(c).ConfigureAwait(false);
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var syntaxNode = root.FindNode(diagnosticSpan);
            var declaration = syntaxNode.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();

            var comments = $@"// add the following directive in autorest.md:
// renaming-mapping:
//   {declaration.Identifier.Text}: {diagnostic.Properties["SuggestedName"]}
";
            var trivia = SyntaxFactory.ParseLeadingTrivia(comments);
            var newNode = syntaxNode.WithLeadingTrivia(syntaxNode.GetLeadingTrivia().InsertRange(0, trivia));
            var newRoot = root.ReplaceNode(syntaxNode, newNode);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
