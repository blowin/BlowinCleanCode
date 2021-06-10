using System;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature
{
    public sealed class ValueReturnNullFeatureSymbolAnalyze : FeatureSymbolAnalyzeBase<IMethodSymbol>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor => Constant.Diagnostic.ReturnNull;

        protected override SymbolKind SymbolKind => SymbolKind.Method;

        protected override void Analyze(SymbolAnalysisContext context, IMethodSymbol symbol)
        {
            if(symbol.ReturnsVoid)
                return;
            
            if(!(symbol.ReturnType is INamedTypeSymbol nts))
                return;

            if(nts.IsValueType || symbol.ReturnNullableAnnotation == NullableAnnotation.Annotated)
                return;
            
            foreach (var reference in symbol.DeclaringSyntaxReferences)
            {
                if(!(reference.GetSyntax(context.CancellationToken) is MethodDeclarationSyntax syntax))
                    continue;

                foreach (var descendantNode in syntax.DescendantNodes())
                {
                    var (isNullStatement, location) = IsNullReturnStatement(descendantNode);
                    if(isNullStatement)
                        ReportDiagnostic(context, location, Array.Empty<object>());
                }
            }
        }

        private static (bool, Location) IsNullReturnStatement(SyntaxNode node)
        {
            if (node is ArrowExpressionClauseSyntax aecs)
                return GetNullLiteralLocation(aecs.Expression);

            if ((node is ReturnStatementSyntax rss))
                return GetNullLiteralLocation(rss.Expression);

            return (false, null);
        }

        private static (bool, Location) GetNullLiteralLocation(ExpressionSyntax es)
        {
            if (es == null)
                return (false, null);

            if (!(es is LiteralExpressionSyntax les))
                return (false, null);

            return (les.IsKind(SyntaxKind.NullLiteralExpression), les.GetLocation());
        }
    }
}