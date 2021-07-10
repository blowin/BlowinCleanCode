using System.Collections.Generic;
using System.Collections.Immutable;
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

            var map = new Dictionary<IdentifierNameSyntax, HashSet<SimpleNameSyntax>>(new SyntaxEqualityComparer());
            foreach (var argumentListArgument in invocation.ArgumentList.Arguments)
            {
                if(!(argumentListArgument.Expression is MemberAccessExpressionSyntax mas))
                    continue;

                var identifier = GetIdentifierNameSyntax(mas);
                if(identifier == null)
                    continue;
                
                if (!map.TryGetValue(identifier, out var arguments))
                {
                    arguments = new HashSet<SimpleNameSyntax>();
                    map.Add(identifier, arguments);
                }

                arguments.Add(mas.Name);
            }
            
            foreach (var pair in map)
            {
                
            }
        }

        private IdentifierNameSyntax GetIdentifierNameSyntax(MemberAccessExpressionSyntax mas)
        {
            switch (mas.Expression)
            {
                case IdentifierNameSyntax ins:
                    return ins;
                case MemberAccessExpressionSyntax mas2:
                    return GetIdentifierNameSyntax(mas2);
                default:
                    return null;
            }
        }
        
        private sealed class SyntaxEqualityComparer : IEqualityComparer<SyntaxNode>
        {
            public bool Equals(SyntaxNode x, SyntaxNode y) => x != null && y != null && x.IsEquivalentTo(x);

            public int GetHashCode(SyntaxNode obj) => obj.GetHashCode();
        }
    }
}