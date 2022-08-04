using System.Collections.Generic;
using System.Linq;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.SingleResponsibility
{
    public sealed class CognitiveComplexityMethodFeatureAnalyze : FeatureSymbolAnalyzeBase<IMethodSymbol>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.CognitiveComplexity, 
            title: "The method has a coherent cognitive complexity.",
            messageFormat: "The method '{0}' has a coherent cognitive complexity.", 
            Constant.Category.SingleResponsibility, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true, 
            description: "The method should be simpler.");

        protected override SymbolKind SymbolKind => SymbolKind.Method;

        protected override void Analyze(SymbolAnalysisContext context, IMethodSymbol ms)
        {
            if(!ms.IsDefinition)
                return;
            
            var walker = new ComplexityWalker(context.Compilation);
            foreach (var reference in ms.DeclaringSyntaxReferences)
            {
                var syntax = reference.GetSyntax(context.CancellationToken) as MethodDeclarationSyntax;
                if(syntax == null)
                    continue;

                var cognitiveComplexity = walker.Complexity(syntax);
                if(cognitiveComplexity > Settings.MaxCognitiveComplexity)
                    ReportDiagnostic(context, syntax.Identifier.GetLocation(), syntax.Identifier.ToString());
            }
        }
    }

    public sealed class ComplexityWalker : CSharpSyntaxWalker
    {
        private int _nesting;
        private int _complexity;
        private Compilation _compilation;
        private SemanticModel _semanticModel;
        private IMethodSymbol _methodSymbol;
        private HashSet<BinaryExpressionSyntax> _alreadeVisitedSyntaxes;

        public ComplexityWalker(Compilation contextCompilation)
        {
            _compilation = contextCompilation;
            _alreadeVisitedSyntaxes = new HashSet<BinaryExpressionSyntax>();
        }

        public int Complexity(CSharpSyntaxNode node)
        {
            _nesting = 0;
            _complexity = 0;
            _alreadeVisitedSyntaxes.Clear();

            (_semanticModel, _methodSymbol) = GetMethodInfo(node, _compilation);

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
                if(symbol.Symbol == null)
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
            IncreaseComplexityIncludeNesting();
            base.VisitParenthesizedLambdaExpression(node);
            _nesting--;
        }

        public override void VisitElseClause(ElseClauseSyntax node)
        {
            _nesting--;
            IncreaseComplexityIncludeNesting();
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
            if (_alreadeVisitedSyntaxes.Contains(node) || !BinaryExpressionExtractor.Support(node))
            {
                base.VisitBinaryExpression(node);
                return;
            }

            // TODO implement BinaryExpressionExtractor
            var flattenedExpressions = node.DescendantNodes().OfType<BinaryExpressionSyntax>();

            IncreaseComplexity();
            _alreadeVisitedSyntaxes.Add(node);

            var prevKind = node.Kind();

            foreach (var currentExpression in flattenedExpressions)
            {
                _alreadeVisitedSyntaxes.Add(currentExpression);

                if (!BinaryExpressionExtractor.Support(currentExpression))
                    continue;

                var currentKind = currentExpression.Kind();
                if (currentKind != prevKind)
                    IncreaseComplexity();

                prevKind = currentKind;
            }

            base.VisitBinaryExpression(node);
        }

        private void IncreaseComplexity() => _complexity++;

        private void IncreaseComplexityIncludeNesting()
        {
            if (_nesting > 1)
            {
                _complexity += (1 + (_nesting - 1));
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

        /// <summary>
        /// Extracts values from left to right
        /// </summary>
        private struct BinaryExpressionExtractor
        {
            private BinaryExpressionSyntax _root;

            public BinaryExpressionExtractor(BinaryExpressionSyntax root)
            {
                _root = root;
            }
            
            public IEnumerable<BinaryExpressionSyntax> Extract()
            {
                if(!Support(_root))
                    yield break;

                if (_root.Left != null)
                {
                    if (_root.Left is BinaryExpressionSyntax binaryExpression)
                    {
                    }
                }

                if (_root.Right != null)
                {

                }
            }

            public static bool Support(BinaryExpressionSyntax syntax) =>
                syntax.Kind().In(SyntaxKind.LogicalOrExpression, SyntaxKind.LogicalAndExpression);
        }
    }
}