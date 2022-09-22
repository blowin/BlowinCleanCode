using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.CodeSmell
{
    public sealed class CatchShouldDoMoreThanRethrowFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase<TryStatementSyntax>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.CatchShouldDoMoreThanRethrow, 
            title: "Catch should do more than rethrow",
            messageFormat: "A catch clause that only rethrows the caught exception has the same effect as omitting the catch altogether and letting it bubble up automatically, but with more code and the additional detriment of leaving maintainers scratching their heads.", 
            Constant.Category.CodeSmell, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true);
        
        protected override SyntaxKind SyntaxKind => SyntaxKind.TryStatement;
        
        protected override void Analyze(SyntaxNodeAnalysisContext context, TryStatementSyntax tryStatementSyntax)
        {
            var locations = GetInvalidLocations(tryStatementSyntax);
            if(locations.IsEmpty)
                return;

            ReportDiagnostic(context, locations[0], locations.Skip(1));
        }

        private ImmutableArray<Location> GetInvalidLocations(TryStatementSyntax tryStatementSyntax)
        {
            if(!ShouldReport(tryStatementSyntax))
                return ImmutableArray<Location>.Empty;
            
            var builder = ImmutableArray.CreateBuilder<Location>();
            foreach (var clauseSyntax in tryStatementSyntax.Catches)
            {
                var throwStatement = GetThrowStatementSyntax(clauseSyntax);
                builder.Add(throwStatement.GetLocation());
            }

            return builder.ToImmutable();
        }

        private bool ShouldReport(TryStatementSyntax tryStatementSyntax)
        {
            foreach (var catchClauseSyntax in tryStatementSyntax.Catches)
            {
                var throwStatement = GetThrowStatementSyntax(catchClauseSyntax);
                if(throwStatement == null || AnalyzerCommentSkipCheck.Skip(throwStatement))
                    return false;

                if (!IsInvalid(throwStatement, catchClauseSyntax))
                    return false;
            }

            return true;
        }

        private static ThrowStatementSyntax GetThrowStatementSyntax(CatchClauseSyntax catchClauseSyntax)
        {
            var statements = catchClauseSyntax.Block?.Statements;
            if(statements == null)
                return null;

            var unwrapStatements = statements.Value;
            if(unwrapStatements.Count != 1)
                return null;

            return unwrapStatements.First() as ThrowStatementSyntax;
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