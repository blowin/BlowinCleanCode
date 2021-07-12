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

            var map = new Dictionary<string, HashSet<SimpleNameSyntax>>();
            foreach (var argumentListArgument in invocation.ArgumentList.Arguments)
            {
                if(!(argumentListArgument.Expression is MemberAccessExpressionSyntax mas))
                    continue;

                var identifier = GetIdentifierNameSyntax(mas);
                var identifierName = identifier?.ToFullString();
                if(string.IsNullOrEmpty(identifierName))
                    continue;
                
                if (!map.TryGetValue(identifierName, out var arguments))
                {
                    arguments = new HashSet<SimpleNameSyntax>();
                    map.Add(identifierName, arguments);
                }

                arguments.Add(mas.Name);
            }

            var preserveWholeObjects = map
                .Where(e => e.Value.Count > Settings.MaxPreserveWholeObjectCount)
                .Select(e => e.Key);

            var argument = string.Join(" and ", preserveWholeObjects);
            
            if(string.IsNullOrEmpty(argument))
                return;
            
            ReportDiagnostic(context, invocation.GetLocation(), argument);
        }

        private static IdentifierNameSyntax GetIdentifierNameSyntax(MemberAccessExpressionSyntax mas)
        {
            var depth = 0;
            const int maxCheckDepth = 256;
            while (true)
            {
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