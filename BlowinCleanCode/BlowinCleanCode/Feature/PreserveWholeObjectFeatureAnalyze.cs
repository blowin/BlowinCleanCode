using System.Collections.Generic;
using System.Linq;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature
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
            var map = new Dictionary<string, HashSet<SimpleNameSyntax>>();
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
                    arguments = new HashSet<SimpleNameSyntax>();
                    map.Add(identifierName, arguments);
                }

                arguments.Add(mas.Name);
            }

            return map.Where(e => e.Value.Count > Settings.MaxPreserveWholeObjectCount)
                .Select(e => e.Key);
        }

        private static bool SkipMethod(InvocationExpressionSyntax invocation, SemanticModel contextSemanticModel)
        {
            // Bad check
            var namespaceName = contextSemanticModel.GetSymbolInfo(invocation).Symbol?.ContainingNamespace?.ContainingModule?.Name ?? string.Empty;
            return namespaceName.StartsWith("System.");
        }

        private static bool IncludeToCheck(MemberAccessExpressionSyntax maes, SemanticModel semanticModel)
        {
            if(!(semanticModel.GetSymbolInfo(maes).Symbol is IFieldSymbol fs))
                return true;

            return !fs.IsStatic && !fs.IsConst;
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