using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature
{
    public abstract class FeatureBase<TSymbol> : IFeature
        where TSymbol : ISymbol
    {
        private string _skipComment;

        public abstract DiagnosticDescriptor DiagnosticDescriptor { get; }

        protected abstract SymbolKind SymbolKind { get; }

        private string SkipComment
        {
            get
            {
                if (_skipComment != null)
                    return _skipComment;

                return (_skipComment = "// Disable " + DiagnosticDescriptor.Id);
            }
        }

        public void Register(AnalysisContext context)
        {
            context.RegisterSymbolAction(AnalyzeWithCheck, SymbolKind);
        }

        private void AnalyzeWithCheck(SymbolAnalysisContext context)
        {
            if(!(context.Symbol is TSymbol s))
                return;
            
            if(SkipAnalyze(context))
                return;

            Analyze(context, s);
        }
        
        protected abstract void Analyze(SymbolAnalysisContext context, TSymbol symbol);

        protected void ReportDiagnostic(SymbolAnalysisContext context, Location location, params object[] args)
        {
            var diagnostic = Diagnostic.Create(DiagnosticDescriptor, location, args);
            context.ReportDiagnostic(diagnostic);
        }

        private bool SkipAnalyze(SymbolAnalysisContext context)
        {
            if (HasSkipComment(context.Symbol, context.CancellationToken))
                return true;
            
            return HasSkipComment(context.Symbol.ContainingSymbol, context.CancellationToken);
        }

        private bool HasSkipComment(ISymbol symbol, CancellationToken cancellationToken)
        {
            foreach (var reference in symbol.DeclaringSyntaxReferences)
            {
                var syntax = reference.GetSyntax(cancellationToken);
                
                if (!syntax.HasLeadingTrivia)
                    continue;

                foreach (var trivia in syntax.GetLeadingTrivia())
                {
                    if (!trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                        continue;

                    if (trivia.ToFullString().Equals(SkipComment))
                        return true;
                }
            }

            return false;
        }
    }
}