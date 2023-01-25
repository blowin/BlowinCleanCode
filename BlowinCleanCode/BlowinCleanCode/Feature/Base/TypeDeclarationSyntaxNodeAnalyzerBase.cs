using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.Base
{
    public abstract class TypeDeclarationSyntaxNodeAnalyzerBase : FeatureSyntaxNodeAnalyzerBase
    {
        public sealed override void Register(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(ctx => AnalyzeWithCheck<TypeDeclarationSyntax>(ctx, Analyze), SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration);
        }

        protected abstract void Analyze(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax syntaxNode);
    }
}
