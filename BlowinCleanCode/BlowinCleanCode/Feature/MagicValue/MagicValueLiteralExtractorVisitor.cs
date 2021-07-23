using System.Collections.Generic;
using System.Linq;
using BlowinCleanCode.Extension;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlowinCleanCode.Feature.MagicValue
{
    internal sealed class MagicValueLiteralExtractorVisitor : CSharpSyntaxVisitor<IEnumerable<LiteralExpressionSyntax>>
    {
        private readonly bool _methodReturnBool;
        private readonly bool _methodReturnTuple;
        private readonly MagicValueSkipSyntaxNodeVisitor _magicValueSkipVisitor;

        public MagicValueLiteralExtractorVisitor(MethodDeclarationSyntax syntax, MagicValueSkipSyntaxNodeVisitor magicValueSkipVisitor)
        {
            _magicValueSkipVisitor = magicValueSkipVisitor;
            _methodReturnBool = MethodReturnBool(syntax);
            _methodReturnTuple = syntax.ReturnType is TupleTypeSyntax;
        }
        
        public override IEnumerable<LiteralExpressionSyntax> VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            if (node.Parent is ArrowExpressionClauseSyntax)
                return Enumerable.Empty<LiteralExpressionSyntax>();
            
            return node.ToSingleEnumerable();
        }

        public override IEnumerable<LiteralExpressionSyntax> VisitArgument(ArgumentSyntax node)
        {
            if (node.NameColon != null && node.Expression is LiteralExpressionSyntax)
                return Enumerable.Empty<LiteralExpressionSyntax>();
            
            return base.VisitArgument(node);
        }

        public override IEnumerable<LiteralExpressionSyntax> VisitReturnStatement(ReturnStatementSyntax node)
        {
            if (!_methodReturnBool && !_methodReturnTuple)
            {
                var invalidItems = node
                    .ChildNodes()
                    .SelectMany(e => e.DescendantNodes(n => !Skip(n)).OfType<LiteralExpressionSyntax>());
                            
                foreach (var literalExpressionSyntax in invalidItems)
                    yield return literalExpressionSyntax;
            }
            else
            {
                foreach (var returnInvalidLiteralNode in GetReturnInvalidLiteralNodes(node, false))
                {
                    if (returnInvalidLiteralNode is LiteralExpressionSyntax rl)
                        yield return rl;
                }   
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
                    .SelectMany(n => n.DescendantNodes(chld => !Skip(chld)).OfType<LiteralExpressionSyntax>());

                foreach (var literalExpressionSyntax in literals)
                    yield return literalExpressionSyntax;
            }
        }
            
        private IEnumerable<LiteralExpressionSyntax> GetReturnInvalidLiteralNodes(SyntaxNode parent, bool canBeInvalid)
        {
            foreach (var syntaxNode in parent.ChildNodes())
            {
                if(Skip(syntaxNode))
                    continue;

                if (syntaxNode is TupleExpressionSyntax && !canBeInvalid)
                    continue;

                if (syntaxNode is LiteralExpressionSyntax literalExpressionSyntax)
                {
                    if (canBeInvalid)
                        yield return literalExpressionSyntax;
                }
                else
                {
                    foreach (var returnInvalidLiteralNode in GetReturnInvalidLiteralNodes(syntaxNode, !(syntaxNode is ConditionalExpressionSyntax)))
                        yield return returnInvalidLiteralNode;
                }
            }
        }
            
        private static bool MethodReturnBool(MethodDeclarationSyntax syntax)
        {
            var kind = syntax.ReturnType.Kind();
            if (syntax.ReturnType is PredefinedTypeSyntax pts)
                kind = pts.Keyword.Kind();

            return kind == SyntaxKind.BoolKeyword;
        }
        
        private bool Skip(SyntaxNode node)
        {
            if (!(node is CSharpSyntaxNode csn))
                return true;

            return csn.Accept(_magicValueSkipVisitor);
        }
    }
}