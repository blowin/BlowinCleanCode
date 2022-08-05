using BlowinCleanCode.Extension.SyntaxExtension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.SingleResponsibility
{
    public sealed class LambdaHaveTooManyLinesFeatureAnalyze : LambdaSyntaxNodeAnalyzerBase
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.LongLambda, 
            title: "Lambda is long",
            messageFormat: "Lambda too long", 
            Constant.Category.SingleResponsibility, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true, 
            description: "Lambda must be shorter");
        
        protected override void Analyze(SyntaxNodeAnalysisContext context, LambdaExpressionSyntax syntaxNode)
        {
            if(syntaxNode.Body == null || AnalyzerCommentSkipCheck.Skip(syntaxNode.Body))
                return;

            var countOfLines = 0;
            foreach (var childNodesAndToken in syntaxNode.Body.ChildNodesAndTokens())
            {
                if (!childNodesAndToken.IsNode) 
                    continue;
                
                countOfLines += childNodesAndToken.AsNode().CountOfLines();
            }
            
            if(countOfLines > Settings.MaxLambdaCountOfLines)
                ReportDiagnostic(context, syntaxNode.GetLocation());
        }
    }
}