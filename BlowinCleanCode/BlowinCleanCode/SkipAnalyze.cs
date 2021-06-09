using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace BlowinCleanCode
{
    public readonly struct SkipAnalyze
    {
        private static readonly Dictionary<string, string> CommentMap = new Dictionary<string, string>();
        
        private readonly DiagnosticDescriptor _descriptor;

        public SkipAnalyze(DiagnosticDescriptor descriptor)
        {
            _descriptor = descriptor;
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
            if (!syntax.HasLeadingTrivia)
                return false;

            var skipComment = GetSkipComment();
            
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
            var skipComment = GetSkipComment();
            
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

        private string GetSkipComment()
        {
            if (!CommentMap.TryGetValue(_descriptor.Id, out var comment))
                comment = "// Disable " + _descriptor.Id;

            return comment;
        }
    }
}