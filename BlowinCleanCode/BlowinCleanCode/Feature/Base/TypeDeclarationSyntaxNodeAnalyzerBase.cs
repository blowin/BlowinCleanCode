using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.Base
{
    public abstract class TypeDeclarationSyntaxNodeAnalyzerBase : FeatureSyntaxNodeAnalyzerBase
    {
        public sealed override void Register(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(ctx => AnalyzeWithCheck<ClassDeclarationSyntax>(ctx, Analyze), SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(ctx => AnalyzeWithCheck<StructDeclarationSyntax>(ctx, Analyze), SyntaxKind.StructDeclaration);
        }

        protected abstract void Analyze(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax syntaxNode);
    }
}