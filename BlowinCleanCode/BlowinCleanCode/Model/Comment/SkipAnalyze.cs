using System.Threading;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Model.Comment.CommentProvider;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlowinCleanCode.Model.Comment
{
    public readonly struct SkipAnalyze
    {
        private readonly DiagnosticDescriptor _descriptor;
        private readonly ICommentProvider _commentProvider;

        public SkipAnalyze(DiagnosticDescriptor descriptor, ICommentProvider commentProvider)
        {
            _descriptor = descriptor;
            _commentProvider = commentProvider;
        }

        public bool Skip(ISymbol symbol, CancellationToken token)
        {
            if (HasSkipComment(symbol, token))
                return true;

            return symbol.ContainingSymbol != null &&
                   !ReferenceEquals(symbol.ContainingSymbol, symbol) &&
                   HasSkipComment(symbol.ContainingSymbol, token);
        }

        public bool Skip(SyntaxNode syntax)
        {
            if (SkipSingleNode(syntax))
                return true;

            foreach (var syntaxNode in syntax.Ancestors())
            {
                switch (syntaxNode)
                {
                    case TypeDeclarationSyntax _:
                        return SkipSingleNode(syntaxNode);
                    case MethodDeclarationSyntax _ when SkipSingleNode(syntaxNode):
                        return true;
                }
            }

            return false;
        }

        private bool SkipSingleNode(SyntaxNode syntax)
        {
            var finder = default(PlaceForCommentFinder);
            syntax = finder.Find(syntax);

            if (!syntax.HasLeadingTrivia)
                return false;

            var skipComment = _commentProvider.SkipComment(_descriptor);

            foreach (var trivia in syntax.GetLeadingTrivia())
            {
                if (!trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                    continue;

                if (trivia.ToFullString().Equals(skipComment))
                    return true;
            }

            return false;
        }

        private bool HasSkipComment(ISymbol symbol, CancellationToken cancellationToken)
        {
            foreach (var reference in symbol.DeclaringSyntaxReferences)
            {
                var syntax = reference.GetSyntax(cancellationToken);

                if (Skip(syntax))
                    return true;
            }

            return false;
        }
    }
}
