using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.Base
{
#pragma warning disable SA1649 // File name should match first type name
    public abstract class FeatureSyntaxNodeAnalyzerBase<TSyntaxNode> : FeatureSyntaxNodeAnalyzerBase
#pragma warning restore SA1649 // File name should match first type name
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
