using System;
using BlowinCleanCode.Comment;
using BlowinCleanCode.Comment.CommentProvider;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.Base
{
    public abstract class FeatureSyntaxNodeAnalyzerBase : IFeature
    {
        public abstract DiagnosticDescriptor DiagnosticDescriptor { get; }
        
        public abstract void Register(AnalysisContext context);
     
        protected AnalyzerSettings Settings => AnalyzerSettings.Instance;
        
        protected SkipAnalyze AnalyzerCommentSkipCheck => new SkipAnalyze(DiagnosticDescriptor, CommentProvider.Instance);
        
        protected void AnalyzeWithCheck<TSyntaxNode>(SyntaxNodeAnalysisContext context, Action<SyntaxNodeAnalysisContext, TSyntaxNode> analyze)
            where TSyntaxNode : SyntaxNode
        {
            if(!(context.Node is TSyntaxNode s))
                return;
            
            var skipAnalyze = AnalyzerCommentSkipCheck;
            if(skipAnalyze.Skip(s))
                return;
            
            analyze(context, s);
        }

        protected void ReportDiagnostic(SyntaxNodeAnalysisContext context, Location location)
            => ReportDiagnostic(context, location, Array.Empty<object>());
        
        protected void ReportDiagnostic(SyntaxNodeAnalysisContext context, Location location, params object[] args)
        {
            var diagnostic = Diagnostic.Create(DiagnosticDescriptor, location, args);
            context.ReportDiagnostic(diagnostic);
        }
    }
    
    public abstract class FeatureSyntaxNodeAnalyzerBase<TSyntaxNode> : FeatureSyntaxNodeAnalyzerBase
        where TSyntaxNode : SyntaxNode
    {
        
        protected abstract SyntaxKind SyntaxKind { get; }

        public override void Register(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(analysisContext => AnalyzeWithCheck<TSyntaxNode>(analysisContext, Analyze), SyntaxKind);
        }

        protected abstract void Analyze(SyntaxNodeAnalysisContext context, TSyntaxNode syntaxNode);
    }
}