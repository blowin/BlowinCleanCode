using System.Collections.Generic;
using System.Linq;
using BlowinCleanCode.Extension;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.CodeSmell.MagicValue
{
    internal sealed class MagicValueLiteralExtractorVisitor : CSharpSyntaxVisitor<IEnumerable<LiteralExpressionSyntax>>
    {
        private readonly MagicValueSkipCheckDescendantNodesVisitor _skipCheckDescendantNodesVisitor;
        private readonly SyntaxNodeAnalysisContext _syntaxNodeAnalysisContext;

        public MagicValueLiteralExtractorVisitor(MagicValueSkipCheckDescendantNodesVisitor skipCheckDescendantNodesVisitor, SyntaxNodeAnalysisContext syntaxNodeAnalysisContext)
        {
            _skipCheckDescendantNodesVisitor = skipCheckDescendantNodesVisitor;
            _syntaxNodeAnalysisContext = syntaxNodeAnalysisContext;
        }

        public override IEnumerable<LiteralExpressionSyntax> VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            if (node.Parent is ArrowExpressionClauseSyntax || SkipCheckDescendantNodes(node))
                return Enumerable.Empty<LiteralExpressionSyntax>();
            
            return node.ToSingleEnumerable();
        }

        // TODO refactor this
        public override IEnumerable<LiteralExpressionSyntax> VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (SkipInvocationExpression(node)) 
                yield break;

            var method = (IMethodSymbol)_syntaxNodeAnalysisContext.SemanticModel.GetSymbolInfo(node).Symbol;
            // TODO check for lambda
            if(method == null)
                yield break;
            
            var realCountOfParameter = method.Parameters.Length;
            // signature
            if(realCountOfParameter <= 1 || 
               (realCountOfParameter <= 2 && method.IsExtensionMethod) || 
               // invocation
               node.ArgumentList.Arguments.Count <= 1)
                yield break;
            
            var withoutFormatParameterCount = realCountOfParameter 
                                              // string argument
                                              - 1 
                                              // Params
                                              - 1;
            var checkWithoutFormatParameterCount = LastIsParams(method) && PenultimateIsString(method);
            
            for (var index = 0; index < node.ArgumentList.Arguments.Count; index++)
            {
                var argument = node.ArgumentList.Arguments[index];
                var isLiteral = argument.Expression is LiteralExpressionSyntax;
                if (argument.NameColon != null && isLiteral)
                    continue;

                if(checkWithoutFormatParameterCount && withoutFormatParameterCount <= index && isLiteral)
                    continue;

                if (isLiteral)
                {
                    yield return (LiteralExpressionSyntax)argument.Expression;
                }
                else
                {
                    foreach (var literalExpressionSyntax in argument.Expression.DescendantNodesAndSelf(n => !SkipCheckDescendantNodes(n)).SelectMany(n => Visit(n)))
                        yield return literalExpressionSyntax;   
                }
            }
        }
        
        public override IEnumerable<LiteralExpressionSyntax> Visit(SyntaxNode node)
        {
            if(SkipCheckDescendantNodes(node))
                return Enumerable.Empty<LiteralExpressionSyntax>();

            return base.Visit(node) ?? Enumerable.Empty<LiteralExpressionSyntax>();
        }
        
        public override IEnumerable<LiteralExpressionSyntax> VisitReturnStatement(ReturnStatementSyntax node)
        {
            foreach (var returnInvalidLiteralNode in GetReturnInvalidLiteralNodes(node, false))
            {
                if (returnInvalidLiteralNode is LiteralExpressionSyntax rl)
                    yield return rl;
            }
        }

        public override IEnumerable<LiteralExpressionSyntax> VisitElementAccessExpression(ElementAccessExpressionSyntax node) => GetReturnInvalidLiteralNodes(node, false);

        public override IEnumerable<LiteralExpressionSyntax> VisitAssignmentExpression(AssignmentExpressionSyntax node) => GetReturnInvalidLiteralNodes(node, false);
            
        public override IEnumerable<LiteralExpressionSyntax> VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            if (node.IsConst || node.Declaration == null)
                yield break;
                
            foreach (var variableDeclaratorSyntax in node.Declaration.Variables)
            {
                var literals = variableDeclaratorSyntax.Initializer.Value
                    .DescendantNodesAndSelf(n => !(n is InvocationExpressionSyntax))
                    .OfType<InvocationExpressionSyntax>()
                    .SelectMany(n => VisitInvocationExpression(n));

                foreach (var literalExpressionSyntax in literals)
                    yield return literalExpressionSyntax;
            }
        }
            
        private bool SkipInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.ArgumentList?.Arguments == null)
                return true;

            if (node.Expression is MemberAccessExpressionSyntax mas)
            {
                if (IsFluent(mas))
                    return true;

                var typeInfo = _syntaxNodeAnalysisContext.SemanticModel.GetTypeInfo(mas.Expression);
                if (typeInfo.Type?.SpecialType == SpecialType.System_String)
                    return true;

                if (mas.OperatorToken.IsKind(SyntaxKind.DotToken) && mas.Name?.Identifier.Text == "ToString")
                    return true;
            }

            return false;
        }
        
        private IEnumerable<LiteralExpressionSyntax> GetReturnInvalidLiteralNodes(SyntaxNode parent, bool canBeInvalid)
        {
            foreach (var syntaxNode in parent.ChildNodes())
            {
                if (SkipCheckDescendantNodes(syntaxNode))
                {
                    if (syntaxNode is CSharpSyntaxNode n)
                    {
                        foreach (var expressionSyntax in n.Accept(this) ?? Enumerable.Empty<LiteralExpressionSyntax>())
                            yield return expressionSyntax;
                    }

                    continue;
                }

                if (syntaxNode is TupleExpressionSyntax && !canBeInvalid)
                    continue;

                if (syntaxNode is LiteralExpressionSyntax literalExpressionSyntax)
                {
                    if (canBeInvalid)
                        yield return literalExpressionSyntax;
                }
                else
                {
                    foreach (var returnInvalidLiteralNode in GetReturnInvalidLiteralNodes(syntaxNode, syntaxNode.IsNot<ConditionalExpressionSyntax>()))
                        yield return returnInvalidLiteralNode;
                }
            }
        }
     
        private bool SkipCheckDescendantNodes(SyntaxNode node) => _skipCheckDescendantNodesVisitor.Visit(node);

        private static bool PenultimateIsString(IMethodSymbol method)
        {
            switch (method.Parameters.Length)
            {
                case 0:
                case 1:
                    return false;
                default:
                    var lastArgument = method.Parameters[method.Parameters.Length - 2];
                    foreach (var lastArgumentDeclaringSyntaxReference in lastArgument.DeclaringSyntaxReferences)
                    {
                        var syntax = lastArgumentDeclaringSyntaxReference.GetSyntax();
                        if (syntax is ParameterSyntax ps && ps.Type is PredefinedTypeSyntax pts && pts.Keyword.IsKind(SyntaxKind.StringKeyword))
                            return true;
                    }
            
                    return false;
            }
        }
        
        private static bool LastIsParams(IMethodSymbol method)
        {
            if (method.Parameters.Length == 0)
                return false;
            
            var lastArgument = method.Parameters[method.Parameters.Length - 1];
            foreach (var lastArgumentDeclaringSyntaxReference in lastArgument.DeclaringSyntaxReferences)
            {
                var syntax = lastArgumentDeclaringSyntaxReference.GetSyntax();
                if (syntax is ParameterSyntax ps && ps.Modifiers.Any(SyntaxKind.ParamsKeyword))
                    return true;
            }
            
            return false;
        }
        
        private bool IsFluent(MemberAccessExpressionSyntax mas)
        {
            if (!(_syntaxNodeAnalysisContext.SemanticModel.GetSymbolInfo(mas.Name).Symbol is IMethodSymbol ms))
                return false;

            return SymbolEqualityComparer.Default.Equals(ms.ContainingType, ms.ReturnType);
        }
    }
}