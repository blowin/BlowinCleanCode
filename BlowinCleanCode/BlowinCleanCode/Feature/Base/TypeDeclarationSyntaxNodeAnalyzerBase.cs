using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.Base
{
    public abstract class TypeDeclarationSyntaxNodeAnalyzerBase : FeatureSyntaxNodeAnalyzerBase
    {
        public sealed override void Register(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(ctx => AnalyzeWithCheck<ClassDeclarationSyntax>(ctx, AnalyzeCore), SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(ctx => AnalyzeWithCheck<StructDeclarationSyntax>(ctx, AnalyzeCore), SyntaxKind.StructDeclaration);
        }

        protected void AnalyzeCore(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax syntaxNode)
        {
            if(AnalyzerCommentSkipCheck.Skip(syntaxNode))
                return;
            
            Analyze(context, syntaxNode);
        }
        
        protected abstract void Analyze(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax syntaxNode);
    }
}