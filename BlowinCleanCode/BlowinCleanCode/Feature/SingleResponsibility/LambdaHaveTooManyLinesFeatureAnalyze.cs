using System.Linq;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.SingleResponsibility
{
    public sealed class LambdaHaveTooManyLinesFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase<SimpleLambdaExpressionSyntax>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.LongLambda, 
            title: "Lambda is long",
            messageFormat: "Lambda too long", 
            Constant.Category.SingleResponsibility, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true, 
            description: "Lambda must be shorter");

        protected override SyntaxKind SyntaxKind => SyntaxKind.SimpleLambdaExpression;
        
        protected override void Analyze(SyntaxNodeAnalysisContext context, SimpleLambdaExpressionSyntax syntaxNode)
        {
            if(syntaxNode.Body == null)
                return;
            
            if(AnalyzerCommentSkipCheck.Skip(syntaxNode.Body))
                return;

            var countOfLines = syntaxNode.Body.ChildNodes().OfType<StatementSyntax>().CountOfLines();
            if(countOfLines > Settings.MaxLambdaCountOfLines)
                ReportDiagnostic(context, syntaxNode.GetLocation());
        }
    }
}