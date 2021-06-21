using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace BlowinCleanCode.Feature
{
    public sealed class ControlFlagFeature : FeatureSyntaxNodeAnalyzerBase<MethodDeclarationSyntax>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.ControlFlag,
            title: "Control Flag",
            messageFormat: "Parameter '{0}' used as a control flag",
            Constant.Category.SingleResponsibility,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        protected override SyntaxKind SyntaxKind => SyntaxKind.MethodDeclaration;

        protected override void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax syntaxNode)
        {
            if (!(syntaxNode.ReturnType is PredefinedTypeSyntax pts))
                return;

            if (pts.Keyword.Kind() == SyntaxKind.BoolKeyword)
                return;

            var flagParameters = FlagParameters(syntaxNode.ParameterList);
            if (flagParameters.Length == 0)
                return;

            var descendant = syntaxNode.Body?.DescendantNodes() ??
                             syntaxNode.ExpressionBody?.DescendantNodes() ??
                             Enumerable.Empty<SyntaxNode>();

            foreach (var controlFlag in UseAsControlFlag(flagParameters, descendant))
                ReportDiagnostic(context, controlFlag.GetLocation(), controlFlag.ToString());
        }

        private ImmutableArray<SyntaxToken> UseAsControlFlag(ImmutableArray<SyntaxToken> flagParameters, IEnumerable<SyntaxNode> body)
        {
            var result = ImmutableArray<SyntaxToken>.Empty;
            foreach (var node in body)
            {
                if (!IsCondition(node))
                    continue;

                var descendant = node.DescendantNodes(sn => !(sn is InvocationExpressionSyntax));
                foreach (var childNode in descendant)
                {
                    if (!(childNode is IdentifierNameSyntax ins))
                        continue;

                    var identifier = ins.Identifier;
                    foreach (var flag in flagParameters)
                    {
                        if (!flag.IsEquivalentTo(identifier)) 
                            continue;
                        
                        result = result.Add(identifier);
                        break;
                    }
                }
            }
            return result;
        }

        private bool IsCondition(SyntaxNode node) => node is ConditionalExpressionSyntax || node is IfStatementSyntax;

        private ImmutableArray<SyntaxToken> FlagParameters(ParameterListSyntax parameterList)
        {
            var countOfParameters = parameterList?.Parameters.Count ?? 0;
            if (countOfParameters == 0)
                return ImmutableArray<SyntaxToken>.Empty;

            var result = ImmutableArray<SyntaxToken>.Empty;
            foreach(var parameter in parameterList.Parameters)
            {
                if (!(parameter.Type is PredefinedTypeSyntax pts))
                    continue;

                if (pts.Keyword.Kind() == SyntaxKind.BoolKeyword)
                    result = result.Add(parameter.Identifier);
            }

            return result;
        }
    }
}
