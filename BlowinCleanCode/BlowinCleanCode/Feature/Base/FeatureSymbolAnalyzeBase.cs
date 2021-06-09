using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.Base
{
    public abstract class FeatureSymbolAnalyzeBase<TSymbol> : IFeature
        where TSymbol : ISymbol
    {
        public abstract DiagnosticDescriptor DiagnosticDescriptor { get; }

        protected abstract SymbolKind SymbolKind { get; }

        public void Register(AnalysisContext context) => context.RegisterSymbolAction(AnalyzeWithCheck, SymbolKind);

        private void AnalyzeWithCheck(SymbolAnalysisContext context)
        {
            if(!(context.Symbol is TSymbol s))
                return;

            var skipAnalyze = new SkipAnalyze(DiagnosticDescriptor);
            if(skipAnalyze.Skip(context.Symbol, context.CancellationToken))
                return;
            
            Analyze(context, s);
        }
        
        protected abstract void Analyze(SymbolAnalysisContext context, TSymbol symbol);

        protected void ReportDiagnostic(SymbolAnalysisContext context, Location location, params object[] args)
        {
            var diagnostic = Diagnostic.Create(DiagnosticDescriptor, location, args);
            context.ReportDiagnostic(diagnostic);
        }
    }
}