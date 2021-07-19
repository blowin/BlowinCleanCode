using System.Linq;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature
{
    public sealed class LargeTypeFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.LargeType, 
            title: "Large type",
            messageFormat: "'{0}' too large", 
            Constant.Category.SingleResponsibility, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true, 
            description: "Type must be shorter");

        public override void Register(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(ctx => AnalyzeWithCheck<ClassDeclarationSyntax>(ctx, Analyze), SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(ctx => AnalyzeWithCheck<StructDeclarationSyntax>(ctx, Analyze), SyntaxKind.StructDeclaration);
        }

        private void Analyze(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax syntaxNode)
        {
            if(AnalyzerCommentSkipCheck.Skip(syntaxNode))
                return;
            
            var (privateCount, nonPrivateCount) = Calculate(syntaxNode);
            
            if (!Settings.LargeClass.IsValid(privateCount, nonPrivateCount))
            {
                var syntaxNodeIdentifier = syntaxNode.Identifier;
                ReportDiagnostic(context, syntaxNodeIdentifier.GetLocation(), syntaxNodeIdentifier.Text);
            }
        }

        private static (int PrivateCount, int NonPrivateCount) Calculate(TypeDeclarationSyntax syntaxNode)
        {
            var privateCount = 0;
            var nonPrivateCount = 0;
            foreach (var methodDeclarationSyntax in syntaxNode.Members.OfType<MethodDeclarationSyntax>())
            {
                var modifiers = methodDeclarationSyntax.Modifiers;
                if (modifiers.Any(SyntaxKind.PublicKeyword) ||
                    modifiers.Any(SyntaxKind.ProtectedKeyword) ||
                    modifiers.Any(SyntaxKind.InternalKeyword)
                )
                {
                    nonPrivateCount += 1;
                }
                else
                {
                    privateCount += 1;
                }
            }

            return (privateCount, nonPrivateCount);
        }
    }
}