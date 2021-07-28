using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlowinCleanCode.Feature.MagicValue
{
    internal sealed class MagicValueSkipSyntaxNodeVisitor : CSharpSyntaxVisitor<bool>
    {
        private readonly SemanticModel _semanticModel;

        public MagicValueSkipSyntaxNodeVisitor(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
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
            
        private bool IsFluent(MemberAccessExpressionSyntax mas)
        {
            if (!(_semanticModel.GetSymbolInfo(mas.Name).Symbol is IMethodSymbol ms))
                return false;

            return SymbolEqualityComparer.Default.Equals(ms.ContainingType, ms.ReturnType);
        }
    }
}