using System.Collections.Generic;
using System.Linq;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Extension.SyntaxExtension;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlowinCleanCode.Model
{
    public sealed class ComplexityWalker : CSharpSyntaxWalker
    {
        private int _nesting;
        private int _complexity;

        private readonly Compilation _compilation;
        private readonly HashSet<SyntaxNode> _visitedSet;

        private SemanticModel _semanticModel;
        private IMethodSymbol _methodSymbol;

        public ComplexityWalker(Compilation contextCompilation)
        {
            _compilation = contextCompilation;
            _visitedSet = new HashSet<SyntaxNode>();
        }

        public int Complexity(CSharpSyntaxNode node)
        {
            _nesting = 0;
            _complexity = 0;
            _visitedSet.Clear();

            (_semanticModel, _methodSymbol) = GetMethodInfo(node, _compilation);

            if (node.ContainsDirectives && node.GetFirstDirective().IsKind(SyntaxKind.IfDirectiveTrivia))
                IncreaseComplexity();

            Visit(node);
            return _complexity;
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (_methodSymbol == null)
            {
                base.VisitInvocationExpression(node);
                return;
            }

            foreach (var descendantNode in node.DescendantNodes(v => !(v is IdentifierNameSyntax)))
            {
                if (!(descendantNode is IdentifierNameSyntax identifierNameSyntax))
                    continue;

                var symbol = _semanticModel.GetSymbolInfo(identifierNameSyntax);
                if (symbol.Symbol == null)
                    continue;

                // Recursive
                if (symbol.Symbol.Equals(_methodSymbol, SymbolEqualityComparer.Default))
                    IncreaseComplexity();
            }

            base.VisitInvocationExpression(node);
        }

        public override void VisitSwitchStatement(SwitchStatementSyntax node)
        {
            _nesting++;
            IncreaseComplexityIncludeNesting();
            base.VisitSwitchStatement(node);
            _nesting--;
        }

        public override void VisitIfStatement(IfStatementSyntax node)
        {
            _nesting++;
            IncreaseComplexityIncludeNesting();
            base.VisitIfStatement(node);
            _nesting--;
        }

        public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            _nesting++;
            base.VisitParenthesizedLambdaExpression(node);
            _nesting--;
        }

        public override void VisitElseClause(ElseClauseSyntax node)
        {
            if (!(node.Statement is IfStatementSyntax))
                IncreaseComplexityIncludeNesting();

            _nesting--;
            base.VisitElseClause(node);
            _nesting++;
        }

        public override void VisitConditionalExpression(ConditionalExpressionSyntax node)
        {
            IncreaseComplexityIncludeNesting();
            base.VisitConditionalExpression(node);
        }

        public override void VisitForStatement(ForStatementSyntax node)
        {
            _nesting++;
            IncreaseComplexityIncludeNesting();
            base.VisitForStatement(node);
            _nesting--;
        }

        public override void VisitForEachStatement(ForEachStatementSyntax node)
        {
            _nesting++;
            IncreaseComplexityIncludeNesting();
            base.VisitForEachStatement(node);
            _nesting--;
        }

        public override void VisitWhileStatement(WhileStatementSyntax node)
        {
            _nesting++;
            IncreaseComplexityIncludeNesting();
            base.VisitWhileStatement(node);
            _nesting--;
        }

        public override void VisitDoStatement(DoStatementSyntax node)
        {
            _nesting++;
            IncreaseComplexityIncludeNesting();
            base.VisitDoStatement(node);
            _nesting--;
        }

        public override void VisitCatchClause(CatchClauseSyntax node)
        {
            _nesting++;
            IncreaseComplexityIncludeNesting();
            base.VisitCatchClause(node);
            _nesting--;
        }

        public override void VisitBreakStatement(BreakStatementSyntax node)
        {
            if (node.Parent.IsNot<SwitchSectionSyntax>())
                IncreaseComplexity();

            base.VisitBreakStatement(node);
        }

        public override void VisitGotoStatement(GotoStatementSyntax node)
        {
            IncreaseComplexity();
            base.VisitGotoStatement(node);
        }

        public override void VisitContinueStatement(ContinueStatementSyntax node)
        {
            IncreaseComplexity();
            base.VisitContinueStatement(node);
        }

        public override void VisitCatchFilterClause(CatchFilterClauseSyntax node)
        {
            IncreaseComplexity();
            base.VisitCatchFilterClause(node);
        }

        public override void VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            var kind = node.Kind();
            if (!kind.In(SyntaxKind.LogicalOrExpression, SyntaxKind.LogicalAndExpression))
            {
                base.VisitBinaryExpression(node);
                return;
            }

            SyntaxKind? prev = null;
            foreach (var flatBinaryExpressionSyntax in node.FlatBinaryExpression(_visitedSet))
            {
                _visitedSet.Add(flatBinaryExpressionSyntax);
                var current = flatBinaryExpressionSyntax.Kind();
                if (prev == null)
                {
                    IncreaseComplexity();
                    prev = current;
                    continue;
                }

                if (prev == current)
                    continue;

                IncreaseComplexity();
                prev = current;
            }

            base.VisitBinaryExpression(node);
        }

        private void IncreaseComplexity() => _complexity++;

        private void IncreaseComplexityIncludeNesting()
        {
            if (_nesting > 1)
            {
                _complexity += 1 + (_nesting - 1);
            }
            else
            {
                _complexity++;
            }
        }

        private static (SemanticModel, IMethodSymbol) GetMethodInfo(CSharpSyntaxNode node, Compilation compilation)
        {
            var methodDeclaration = node as MethodDeclarationSyntax ?? node.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();

            if (methodDeclaration == null)
                return (null, null);

            var semanticModel = compilation.GetSemanticModel(node.SyntaxTree);
            return (semanticModel, semanticModel?.GetDeclaredSymbol(methodDeclaration));
        }
    }
}