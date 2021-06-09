using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.Base
{
    public abstract class FeatureSyntaxNodeAnalyzerBase<TSyntaxNode> : IFeature
        where TSyntaxNode : SyntaxNode
    {
        public abstract DiagnosticDescriptor DiagnosticDescriptor { get; }
        
        protected abstract SyntaxKind SyntaxKind { get; }

        public void Register(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeWithCheck, SyntaxKind);
        }

        private void AnalyzeWithCheck(SyntaxNodeAnalysisContext context)
        {
            if(!(context.Node is TSyntaxNode s))
                return;
            
            var symbol = context.SemanticModel.GetDeclaredSymbol(context.Node, context.CancellationToken);
            var skipAnalyze = new SkipAnalyze(DiagnosticDescriptor);
            if(skipAnalyze.Skip(context.ContainingSymbol, context.CancellationToken))
                return;
            
            Analyze(context, s);
        }

        protected abstract void Analyze(SyntaxNodeAnalysisContext context, TSyntaxNode syntaxNode);
        
        protected void ReportDiagnostic(SyntaxNodeAnalysisContext context, Location location, params object[] args)
        {
            var diagnostic = Diagnostic.Create(DiagnosticDescriptor, location, args);
            context.ReportDiagnostic(diagnostic);
        }
    }
}