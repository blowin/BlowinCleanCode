using System.Collections.Immutable;
using System.Linq;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Extension.SyntaxExtension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.SingleResponsibility
{
    public sealed class ControlFlagFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase<MethodDeclarationSyntax>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(
            Constant.Id.ControlFlag,
            title: "Control Flag",
            messageFormat: "Parameter '{0}' used as a control flag",
            Constant.Category.SingleResponsibility,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        protected override SyntaxKind SyntaxKind => SyntaxKind.MethodDeclaration;

        protected override void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax syntaxNode)
        {
            if (!(syntaxNode.ReturnType is PredefinedTypeSyntax pts) || pts.IsBool())
                return;

            var flagParameters = FlagParameters(syntaxNode.ParameterList);
            if (flagParameters.Length == 0)
                return;

            var descendant = syntaxNode.Body ?? (SyntaxNode)syntaxNode.ExpressionBody;

            foreach (var controlFlag in UseAsControlFlag(flagParameters, descendant, pts.Keyword.Kind()))
            {
                if (!AnalyzerCommentSkipCheck.Skip(controlFlag.Parent))
                    ReportDiagnostic(context, controlFlag.GetLocation(), controlFlag.Text);
            }
        }

        private static ImmutableArray<SyntaxToken> UseAsControlFlag(ImmutableArray<string> flagParameters, SyntaxNode body, SyntaxKind returnKeyword)
        {
            if (body == null)
                return ImmutableArray<SyntaxToken>.Empty;

            var result = ImmutableArray<SyntaxToken>.Empty;
            foreach (var node in body.DescendantNodes())
            {
                var condition = GetCondition(node);
                if (condition == null)
                    continue;

                var descendant = condition
                    .DescendantNodesAndSelf(sn => sn.IsAny<BinaryExpressionSyntax, ParenthesizedExpressionSyntax>())
                    .OfType<IdentifierNameSyntax>();

                foreach (var nameSyntax in descendant)
                {
                    if (ConditionForSingleReturn(nameSyntax, returnKeyword))
                        continue;

                    var identifier = nameSyntax.Identifier;
                    var fieldName = identifier.Text;
                    foreach (var flag in flagParameters)
                    {
                        if (flag != fieldName)
                            continue;

                        result = result.Add(identifier);
                        break;
                    }
                }
            }

            return result;
        }

        private static ExpressionSyntax GetCondition(SyntaxNode node)
        {
            if (node is IfStatementSyntax ifStatement)
                return ifStatement.Condition;

            if (node is ConditionalExpressionSyntax conditionalExpression)
                return conditionalExpression.Condition;

            return null;
        }

        private static bool ConditionForSingleReturn(SyntaxNode nameSyntax, SyntaxKind returnKeyword)
        {
            // Only for void method
            if (returnKeyword != SyntaxKind.VoidKeyword)
                return false;

            foreach (var syntaxNode in nameSyntax.Ancestors())
            {
                if (!syntaxNode.Is<IfStatementSyntax>(out var ifStatement))
                    return false;

                switch (ifStatement.Statement)
                {
                    case ReturnStatementSyntax _:
                        return true;
                    case BlockSyntax blockSyntax:
                        return blockSyntax.Statements.FirstOrDefault() is ReturnStatementSyntax;
                }

                return false;
            }

            return false;
        }

        private static ImmutableArray<string> FlagParameters(ParameterListSyntax parameterList)
        {
            if (parameterList?.Parameters == null)
                return ImmutableArray<string>.Empty;

            if (parameterList.Parameters.Count == 0)
                return ImmutableArray<string>.Empty;

            var result = ImmutableArray<string>.Empty;
            foreach (var parameter in parameterList.Parameters)
            {
                if (!(parameter.Type is PredefinedTypeSyntax pts))
                    continue;

                if (pts.Keyword.IsKind(SyntaxKind.BoolKeyword))
                    result = result.Add(parameter.Identifier.Text);
            }

            return result;
        }
    }
}
