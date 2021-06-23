using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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
                ReportDiagnostic(context, controlFlag.GetLocation(), controlFlag.Text);
        }

        private static ImmutableArray<SyntaxToken> UseAsControlFlag(ImmutableArray<string> flagParameters,
            IEnumerable<SyntaxNode> body)
        {
            var result = ImmutableArray<SyntaxToken>.Empty;
            foreach (var node in body.Where(node => node is IfStatementSyntax || node is ConditionalExpressionSyntax))
            {
                var descendant = node
                    .DescendantNodes(sn => !(sn is InvocationExpressionSyntax))
                    .OfType<IdentifierNameSyntax>();
                
                foreach (var nameSyntax in descendant)
                {
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

        private static ImmutableArray<string> FlagParameters(ParameterListSyntax parameterList)
        {
            var countOfParameters = parameterList?.Parameters.Count ?? 0;
            if (countOfParameters == 0)
                return ImmutableArray<string>.Empty;

            var result = ImmutableArray<string>.Empty;
            foreach(var parameter in parameterList.Parameters)
            {
                if (!(parameter.Type is PredefinedTypeSyntax pts))
                    continue;

                if (pts.Keyword.Kind() == SyntaxKind.BoolKeyword)
                    result = result.Add(parameter.Identifier.Text);
            }

            return result;
        }
    }
}
