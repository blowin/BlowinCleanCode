using System.Threading;
using BlowinCleanCode.Comment.CommentProvider;
using BlowinCleanCode.Extension;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace BlowinCleanCode.Comment
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
            var finder = new PlaceForCommentFinder();
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
            var skipComment = _commentProvider.SkipComment(_descriptor);
            
            foreach (var reference in symbol.DeclaringSyntaxReferences)
            {
                var syntax = reference.GetSyntax(cancellationToken);
                
                if (!syntax.HasLeadingTrivia)
                    continue;

                foreach (var trivia in syntax.GetLeadingTrivia())
                {
                    if (!trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                        continue;

                    if (trivia.ToFullString().Equals(skipComment))
                        return true;
                }
            }

            return false;
        }
    }
}