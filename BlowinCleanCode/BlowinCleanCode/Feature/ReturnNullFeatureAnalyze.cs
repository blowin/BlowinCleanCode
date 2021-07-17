using System;
using System.Linq;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature
{
    public sealed class ReturnNullFeatureSymbolAnalyze : FeatureSymbolAnalyzeBase<IMethodSymbol>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.ReturnNull, 
            title: "Method return null",
            messageFormat: "Return statement with null", 
            Constant.Category.GoodPractice, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true, 
            description: "Return null bad practice. Use null object pattern");

        protected override SymbolKind SymbolKind => SymbolKind.Method;

        protected override void Analyze(SymbolAnalysisContext context, IMethodSymbol symbol)
        {
            if (Skip(symbol)) 
                return;

            foreach (var reference in symbol.DeclaringSyntaxReferences)
            {
                if(!(reference.GetSyntax(context.CancellationToken) is MethodDeclarationSyntax syntax))
                    continue;

                // string Run() => null;
                var expr = syntax.ExpressionBody?.Expression;
                if (expr != null && IsNull(expr))
                {
                    ReportDiagnostic(context, expr.GetLocation());
                    return;
                }
                
                foreach (var returnStatementSyntax in syntax.DescendantNodes().OfType<ReturnStatementSyntax>())
                {
                    if(AnalyzerCommentSkipCheck.Skip(returnStatementSyntax))
                        continue;
                    
                    var location = GetNullLiteralLocation(returnStatementSyntax.Expression);
                    if(location != null)
                        ReportDiagnostic(context, location);
                }
            }
        }

        private static bool Skip(IMethodSymbol symbol)
        {
            if (symbol.ReturnsVoid)
                return true;

            if (!(symbol.ReturnType is INamedTypeSymbol nts))
                return true;

            if (nts.IsValueType || symbol.ReturnNullableAnnotation == NullableAnnotation.Annotated)
                return true;
            
            return false;
        }

        private static Location GetNullLiteralLocation(ExpressionSyntax es)
        {
            if (IsNull(es))
                return es.GetLocation();
            
            var nullNode = es?
                .DescendantNodes(e => e is ConditionalExpressionSyntax)
                .FirstOrDefault(e => IsNull(e));

            return nullNode?.GetLocation();
        }

        private static bool IsNull(SyntaxNode node) => node.IsKind(SyntaxKind.NullLiteralExpression);
    }
}