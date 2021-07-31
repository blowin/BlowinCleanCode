using System.Linq;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.GoodPractice
{
    public sealed class FinalizersShouldNotBeEmptyFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase<DestructorDeclarationSyntax>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.FinalizersShouldNotBeEmpty, 
            title: "Finalizers should not be empty",
            messageFormat: "Finalizers should not be empty", 
            Constant.Category.GoodPractice, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true, 
            description: "Finalizers come with a performance cost due to the overhead of tracking the life cycle of objects. An empty one is consequently costly with no benefit or justification.");
        
        protected override SyntaxKind SyntaxKind => SyntaxKind.DestructorDeclaration;
        
        protected override void Analyze(SyntaxNodeAnalysisContext context, DestructorDeclarationSyntax syntaxNode)
        {
            if(syntaxNode.ExpressionBody != null)
                return;
            
            var count = syntaxNode.Body?.Statements.Count(s => !(s is ReturnStatementSyntax)) ?? 0;
            if(count > 0)
                return;
            
            ReportDiagnostic(context, syntaxNode.GetLocation());
        }
    }
}