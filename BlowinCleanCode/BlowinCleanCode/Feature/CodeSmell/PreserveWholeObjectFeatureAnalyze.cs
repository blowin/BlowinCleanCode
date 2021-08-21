using System.Collections.Generic;
using System.Linq;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.CodeSmell
{
    public sealed class PreserveWholeObjectFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase<InvocationExpressionSyntax>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.PreserveWholeObject, 
            title: "Preserve whole object",
            messageFormat: "Preserve whole object '{0}'", 
            Constant.Category.CodeSmell,
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true);

        protected override SyntaxKind SyntaxKind => SyntaxKind.InvocationExpression;
        
        protected override void Analyze(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation)
        {
            if(invocation.ArgumentList == null)
                return;

            if(SkipMethod(invocation, context.SemanticModel))
                return;
            
            var preserveWholeObjects = AllInvalidItems(context, invocation);
            var argument = string.Join(" and ", preserveWholeObjects);
            
            if(string.IsNullOrEmpty(argument))
                return;
            
            ReportDiagnostic(context, invocation.GetLocation(), argument);
        }

        private IEnumerable<string> AllInvalidItems(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation)
        {
            var map = new Dictionary<string, Box<int>>();
            foreach (var argumentListArgument in invocation.ArgumentList.Arguments)
            {
                if (!(argumentListArgument.Expression is MemberAccessExpressionSyntax mas))
                    continue;

                var identifier = GetIdentifierNameSyntax(mas, context.SemanticModel);
                var identifierName = identifier?.ToFullString();
                if (string.IsNullOrEmpty(identifierName))
                    continue;

                if (!map.TryGetValue(identifierName, out var arguments))
                {
                    arguments = new Box<int>(1);
                    map.Add(identifierName, arguments);
                }
                else
                {
                    arguments.Value += 1;
                }
            }

            return map.Where(e => e.Value.Value > Settings.MaxPreserveWholeObjectCount).Select(e => e.Key);
        }

        private static bool SkipMethod(InvocationExpressionSyntax invocation, SemanticModel contextSemanticModel)
        {
            // Bad check
            var namespaceName = contextSemanticModel.GetSymbolInfo(invocation).Symbol?.ContainingNamespace?.ContainingModule?.Name ?? string.Empty;
            return namespaceName.StartsWith("System.");
        }

        private static bool IncludeToCheck(MemberAccessExpressionSyntax maes, SemanticModel semanticModel)
        {
            var symbol = semanticModel.GetSymbolInfo(maes).Symbol;
            return symbol.Is<IFieldSymbol>(out var fs) && !fs.IsStatic && !fs.IsConst;
        }

        private static IdentifierNameSyntax GetIdentifierNameSyntax(MemberAccessExpressionSyntax mas, SemanticModel semanticModel)
        {
            var depth = 0;
            const int maxCheckDepth = 256;
            while (true)
            {
                if (!IncludeToCheck(mas, semanticModel))
                    return null;
                
                if(depth == maxCheckDepth)
                    break;
                
                switch (mas.Expression)
                {
                    case IdentifierNameSyntax ins:
                        return ins;
                    case MemberAccessExpressionSyntax mas2:
                        mas = mas2;
                        depth += 1;
                        continue;
                    default:
                        return null;
                }
            }

            return null;
        }
    }
}