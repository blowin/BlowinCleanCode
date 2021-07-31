using System.Linq;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.SingleResponsibility
{
    public sealed class MethodALotOfDeclarationFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase<MethodDeclarationSyntax>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.MethodContainALotOfDeclaration, 
            title: "Method has a lot of declaration",
            messageFormat: "Method '{0}' has a lot of declaration {1}/{2}", 
            Constant.Category.SingleResponsibility, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true);
        
        protected override SyntaxKind SyntaxKind => SyntaxKind.MethodDeclaration;
        
        protected override void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax syntaxNode)
        {
            var countOfDeclarations = syntaxNode
                .DescendantNodes()
                .OfType<LocalDeclarationStatementSyntax>()
                .Count(e => !e.IsConst);

            if (countOfDeclarations > Settings.MaxMethodDeclaration)
                ReportDiagnostic(context, syntaxNode.Identifier.GetLocation(), syntaxNode.Identifier.Text, countOfDeclarations, Settings.MaxMethodDeclaration);
        }
    }
}