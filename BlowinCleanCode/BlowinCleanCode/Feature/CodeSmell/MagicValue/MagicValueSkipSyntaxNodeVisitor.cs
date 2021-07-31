using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlowinCleanCode.Feature.CodeSmell.MagicValue
{
    internal sealed class MagicValueSkipSyntaxNodeVisitor : CSharpSyntaxVisitor<bool>
    {
        public override bool VisitMethodDeclaration(MethodDeclarationSyntax node) => node.Identifier.ValueText == nameof(GetHashCode);

        public override bool VisitInvocationExpression(InvocationExpressionSyntax node) => true;

        public override bool VisitEqualsValueClause(EqualsValueClauseSyntax node)
        {
            //                 ↓
            // Run(int v, bool = true)
            return node.Value is LiteralExpressionSyntax;
        }

        public override bool VisitCaseSwitchLabel(CaseSwitchLabelSyntax node) => true;

        public override bool VisitAttributeArgument(AttributeArgumentSyntax node) => true;

        public override bool VisitReturnStatement(ReturnStatementSyntax node) => true;

        public override bool VisitElementAccessExpression(ElementAccessExpressionSyntax node) => true;

        public override bool VisitAssignmentExpression(AssignmentExpressionSyntax node) => true;

        public override bool VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node) => true;

        public override bool VisitArgument(ArgumentSyntax node) => true;
    }
}