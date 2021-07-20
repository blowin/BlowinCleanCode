using System;
using System.Collections.Generic;
using System.Linq;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature
{
    public sealed class MagicValueFeatureSymbolAnalyze : FeatureSyntaxNodeAnalyzerBase<MethodDeclarationSyntax>
    {
        private static readonly List<string> SkipLiterals = new List<string>
        {
            "0",
            "1",
            "-1",
        };
        
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(
            Constant.Id.MagicValue,
            title: "Expression shouldn't contain magic value",
            messageFormat: "Magic value '{0}'",
            Constant.Category.CodeSmell,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        protected override SyntaxKind SyntaxKind => SyntaxKind.MethodDeclaration;

        protected override void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax syntax)
        {
            var visitor = new SkipSyntaxNodeVisitor(syntax, context.SemanticModel);
            foreach (var syntaxNode in InvalidNodes(syntax, visitor))
                ReportDiagnostic(context, syntaxNode.GetLocation(), syntaxNode.ToFullString());
        }
        
        private IEnumerable<SyntaxNode> InvalidNodes(MethodDeclarationSyntax syntax, SkipSyntaxNodeVisitor skipVisitor)
        {
            foreach (var literal in syntax.DescendantNodes(n => !Skip(n, skipVisitor)).OfType<LiteralExpressionSyntax>())
            {
                if (AnalyzerCommentSkipCheck.Skip(literal))
                    continue;
                
                if(literal.IsKind(SyntaxKind.NullLiteralExpression))
                    continue;

                if(SkipLiterals.Contains(literal.Token.ValueText ?? string.Empty))
                    continue;
                
                yield return literal;
            }
        }

        private bool Skip(SyntaxNode node, SkipSyntaxNodeVisitor skipVisitor)
        {
            if (!(node is CSharpSyntaxNode csn))
                return true;

            return csn.Accept(skipVisitor);
        }
        
        private sealed class SkipSyntaxNodeVisitor : CSharpSyntaxVisitor<bool>
        {
            private readonly bool _methodReturnBool;
            private readonly bool _methodReturnNamedTuple;
            private readonly SemanticModel _semanticModel;

            public SkipSyntaxNodeVisitor(MethodDeclarationSyntax methodSymbol, SemanticModel semanticModel)
            {
                _methodReturnBool = MethodReturnBool(methodSymbol);
                _methodReturnNamedTuple = MethodReturnNamedTuple(methodSymbol);
                _semanticModel = semanticModel;
            }

            public override bool VisitReturnStatement(ReturnStatementSyntax node)
            {
                if (_methodReturnNamedTuple)
                    return true;

                if (!_methodReturnBool || !(node.Expression is LiteralExpressionSyntax les))
                    return false;

                switch (les.Kind())
                {
                    case SyntaxKind.TrueLiteralExpression:
                    case SyntaxKind.FalseLiteralExpression:
                        return true;
                    default:
                        return false;
                }
            }

            public override bool VisitInvocationExpression(InvocationExpressionSyntax node)
            {
                if (node.Expression is MemberAccessExpressionSyntax mas)
                {
                    if (IsFluent(mas))
                        return true;

                    var typeInfo = _semanticModel.GetTypeInfo(mas.Expression);
                    if (typeInfo.Type?.SpecialType == SpecialType.System_String)
                        return true;

                    if (mas.OperatorToken.IsKind(SyntaxKind.DotToken))
                        return mas.Name?.Identifier.Text == "ToString";
                }

                return base.VisitInvocationExpression(node);
            }

            public override bool VisitElementAccessExpression(ElementAccessExpressionSyntax node) => true;

            public override bool VisitArgument(ArgumentSyntax node) => node.NameColon != null;

            public override bool VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node) =>
                node.IsConst;

            public override bool VisitVariableDeclaration(VariableDeclarationSyntax node)
            {
                foreach (var variableDeclaratorSyntax in node.Variables)
                {
                    var initializeValue = variableDeclaratorSyntax.Initializer.Value;
                    if (initializeValue.IsKind(SyntaxKind.InvocationExpression))
                        return false;
                }

                return true;
            }

            private bool IsFluent(MemberAccessExpressionSyntax mas)
            {
                if (!(_semanticModel.GetSymbolInfo(mas.Name).Symbol is IMethodSymbol ms))
                    return false;

                return SymbolEqualityComparer.Default.Equals(ms.ContainingType, ms.ReturnType);
            }

            private static bool MethodReturnBool(MethodDeclarationSyntax syntax)
            {
                var kind = syntax.ReturnType.Kind();
                if (syntax.ReturnType is PredefinedTypeSyntax pts)
                    kind = pts.Keyword.Kind();

                return kind == SyntaxKind.BoolKeyword;
            }

            private bool MethodReturnNamedTuple(MethodDeclarationSyntax methodSymbol)
            {
                return methodSymbol.ReturnType is TupleTypeSyntax;
            }
        }
    }
}
