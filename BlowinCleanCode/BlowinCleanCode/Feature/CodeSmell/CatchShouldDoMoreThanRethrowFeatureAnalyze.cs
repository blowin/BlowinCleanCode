using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.CodeSmell
{
    public sealed class CatchShouldDoMoreThanRethrowFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase<CatchClauseSyntax>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.CatchShouldDoMoreThanRethrow, 
            title: "Catch should do more than rethrow",
            messageFormat: "A catch clause that only rethrows the caught exception has the same effect as omitting the catch altogether and letting it bubble up automatically, but with more code and the additional detriment of leaving maintainers scratching their heads.", 
            Constant.Category.CodeSmell, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true);
        
        protected override SyntaxKind SyntaxKind => SyntaxKind.CatchClause;
        
        protected override void Analyze(SyntaxNodeAnalysisContext context, CatchClauseSyntax syntaxNode)
        {
            var statements = syntaxNode.Block?.Statements;
            if(statements == null)
                return;

            var unwrapStatements = statements.Value;
            if(unwrapStatements.Count != 1)
                return;

            var throwStatement = unwrapStatements.First() as ThrowStatementSyntax;
            if(throwStatement == null || AnalyzerCommentSkipCheck.Skip(throwStatement))
                return;
            
            if(IsInvalid(throwStatement, syntaxNode))
                ReportDiagnostic(context, throwStatement.GetLocation());
        }

        private static bool IsInvalid(ThrowStatementSyntax throwExpressionSyntax, CatchClauseSyntax syntaxNode)
        {
            switch (throwExpressionSyntax.Expression)
            {
                case null:
                    return true;
                case IdentifierNameSyntax identifierNameSyntax:
                    var catchIdentifier = syntaxNode.Declaration?.Identifier;
                    return catchIdentifier != null && identifierNameSyntax.Identifier.IsEquivalentTo(catchIdentifier.Value);
                default:
                    return false;
            }
        }
    }
}