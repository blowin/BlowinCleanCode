using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlowinCleanCode.Feature.MagicValue
{
    internal sealed class MagicValueSkipSyntaxNodeVisitor : CSharpSyntaxVisitor<bool>
    {
        public override bool Visit(SyntaxNode node)
        {
            var methodDeclarationSyntax = node.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            if (methodDeclarationSyntax != null && methodDeclarationSyntax.Identifier.ValueText == nameof(GetHashCode))
                return true;
            
            return base.Visit(node);
        }

        public override bool VisitInvocationExpression(InvocationExpressionSyntax node) => true;

        public override bool VisitEqualsValueClause(EqualsValueClauseSyntax node)
        {
            //                 ↓
            // Run(int v, bool = true)
            return node.Value is LiteralExpressionSyntax;
        }

        public override bool VisitAttributeArgument(AttributeArgumentSyntax node) => true;

        public override bool VisitReturnStatement(ReturnStatementSyntax node) => true;

        public override bool VisitElementAccessExpression(ElementAccessExpressionSyntax node) => true;

        public override bool VisitAssignmentExpression(AssignmentExpressionSyntax node) => true;

        public override bool VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node) => true;

        public override bool VisitArgument(ArgumentSyntax node) => true;
    }
}