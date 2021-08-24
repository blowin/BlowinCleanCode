using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.Base
{
    public abstract class LambdaSyntaxNodeAnalyzerBase : FeatureSyntaxNodeAnalyzerBase
    {
        public override void Register(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(ctx => AnalyzeWithCheck<SimpleLambdaExpressionSyntax>(ctx, Analyze), SyntaxKind.SimpleLambdaExpression);
            context.RegisterSyntaxNodeAction(ctx => AnalyzeWithCheck<ParenthesizedLambdaExpressionSyntax>(ctx, Analyze), SyntaxKind.ParenthesizedLambdaExpression);
        }
   
        protected abstract void Analyze(SyntaxNodeAnalysisContext context, LambdaExpressionSyntax syntaxNode);
    }
}