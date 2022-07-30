using System.Linq;
using BlowinCleanCode.Extension.SyntaxExtension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.SingleResponsibility
{
    public sealed class LargeTypeFeatureAnalyze : TypeDeclarationSyntaxNodeAnalyzerBase
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.LargeType, 
            title: "Large type",
            messageFormat: "'{0}' too large", 
            Constant.Category.SingleResponsibility, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true, 
            description: "Type must be shorter");

        protected override void Analyze(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax syntaxNode)
        {
            var (privateCount, nonPrivateCount) = Calculate(syntaxNode);
            
            if (!Settings.LargeClass.IsValid(privateCount, nonPrivateCount))
            {
                ReportDiagnostic(context, syntaxNode.Identifier.GetLocation(), syntaxNode.TypeName());
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