using System.Linq;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature
{
    public sealed class LargeClassFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase<ClassDeclarationSyntax>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.LargeClass, 
            title: "Large class",
            messageFormat: "'{0}' too large", 
            Constant.Category.SingleResponsibility, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true, 
            description: "Class must be shorter");

        protected override SyntaxKind SyntaxKind => SyntaxKind.ClassDeclaration;
        
        protected override void Analyze(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax syntaxNode)
        {
            var (privateCount, nonPrivateCount) = Calculate(syntaxNode);
            
            if (!Settings.LargeClass.IsValid(privateCount, nonPrivateCount))
            {
                var syntaxNodeIdentifier = syntaxNode.Identifier;
                ReportDiagnostic(context, syntaxNodeIdentifier.GetLocation(), syntaxNodeIdentifier.Text);
            }
        }

        private static (int PrivateCount, int NonPrivateCount) Calculate(ClassDeclarationSyntax syntaxNode)
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