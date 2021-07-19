using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlowinCleanCode.Comment;
using BlowinCleanCode.Comment.CommentProvider;
using BlowinCleanCode.Extension;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Document = Microsoft.CodeAnalysis.Document;

namespace BlowinCleanCode.CodeFix
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DisableWithCommentCodeFixProvider)), Shared]
    public class DisableWithCommentCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = BuildFixableDiagnosticIds();
        
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if(root == null)
                return;

            foreach (var diagnostic in context.Diagnostics.Where(e => Constant.ListOf.Id.Contains(e.Id)))
            {
                var node = root.FindNode(diagnostic.Location.SourceSpan);
                if(node == null)
                    continue;

                var title = ActionTitle(diagnostic, string.Empty);
                var codeAction = FixCodeAction(title, diagnostic, context, node);
                var codeActions = ImmutableArray<CodeAction>.Empty.Add(codeAction);
                
                foreach (var parentNode in node.Ancestors())
                {
                    switch (parentNode)
                    {
                        case MethodDeclarationSyntax md:
                            title = ActionTitle(diagnostic, " for method");
                            codeAction = FixCodeAction(title, diagnostic, context, md);
                            codeActions = codeActions.Add(codeAction);
                            break;
                        case ClassDeclarationSyntax cd:
                            title = ActionTitle(diagnostic, " for class");
                            codeAction = FixCodeAction(title, diagnostic, context, cd);
                            codeActions = codeActions.Add(codeAction);
                            break;
                    }
                }
                
                context.RegisterCodeFix(
                        CodeAction.Create($"Disable '{diagnostic.Descriptor.Title}'", codeActions, true),
                        diagnostic
                );
            }
        }

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        protected string ActionTitle(Diagnostic diagnostic, string postfix) => $"Disable '{diagnostic.Descriptor.Title}' with comment" + postfix;

        protected CodeAction FixCodeAction(string title, Diagnostic diagnostic, CodeFixContext context, SyntaxNode node)
        {
            return CodeAction.Create(title, 
                token => AddComment(context.Document, node, diagnostic, token), 
                CommentProvider.Instance.SkipComment(diagnostic));
        }

        private async Task<Document> AddComment(Document document, SyntaxNode node, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            node = new PlaceForCommentFinder().Find(node);
            
            var updateNode = WithLeadingTrivia(node, diagnostic);
            
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (syntaxRoot == null)
                return document;
            
            var newSyntaxRoot = syntaxRoot.ReplaceNode(node, updateNode);

            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static SyntaxNode WithLeadingTrivia(SyntaxNode node, Diagnostic diagnostic)
        {
            var list = node.GetLeadingTrivia();

            var lastSpaces = list.Reverse()
                .TakeWhile(e => e.IsKind(SyntaxKind.WhitespaceTrivia))
                .ToImmutableArray();

            var comment = CommentProvider.Instance.SkipComment(diagnostic);
            
            var trivia = list.Add(SyntaxFactory.Comment(comment + Environment.NewLine))
                .AddRange(lastSpaces);

            return node.WithLeadingTrivia(trivia);
        }
        
        private static ImmutableArray<string> BuildFixableDiagnosticIds()
        {
            var res = ImmutableArray.CreateBuilder<string>();
            foreach (var s in Constant.ListOf.Id)
                res.Add(s);

            return res.ToImmutable();
        }
    }
}