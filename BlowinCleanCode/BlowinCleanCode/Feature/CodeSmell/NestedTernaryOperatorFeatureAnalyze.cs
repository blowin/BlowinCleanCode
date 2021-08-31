using System.Linq;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.CodeSmell
{
    public sealed class NestedTernaryOperatorFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase<MethodDeclarationSyntax>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.NestedTernaryOperator, 
            title: "Ternary operators should not be nested",
            messageFormat: "Ternary operators should not be nested, use another line to express the nested operation as a separate statement", 
            Constant.Category.CodeSmell, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true, 
            description: "Just because you can do something, doesn't mean you should, and that's the case with nested ternary operations. Nesting ternary operators results in the kind of code that may seem clear as day when you write it, but six months later will leave maintainers (or worse - future you) scratching their heads and cursing.");
        
        protected override SyntaxKind SyntaxKind => SyntaxKind.MethodDeclaration;
        
        protected override void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax syntaxNode)
        {
            foreach (var conditionalExpressionSyntax in syntaxNode.DescendantNodes(e => e.IsNot<ConditionalExpressionSyntax>()).OfType<ConditionalExpressionSyntax>())
            {
                foreach (var childConditionalExpressionSyntax in conditionalExpressionSyntax.DescendantNodes().OfType<ConditionalExpressionSyntax>())
                {
                    if(AnalyzerCommentSkipCheck.Skip(childConditionalExpressionSyntax))
                        continue;
                    
                    ReportDiagnostic(context, childConditionalExpressionSyntax.GetLocation());
                }
            }
        }
    }
}