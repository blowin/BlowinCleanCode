using System.Collections.Generic;
using System.Linq;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature
{
    public class LongChainCallFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase<MethodDeclarationSyntax>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.LongChainCall, 
            title: "Too many chained references",
            messageFormat: "Too many chained references", 
            Constant.Category.SingleResponsibility, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true);

        protected override SyntaxKind SyntaxKind => SyntaxKind.MethodDeclaration;
        
        protected override void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax syntaxNode)
        {
            var checkInvocationExpressions = syntaxNode
                // Don't check child calls
                .DescendantNodes(i => !(i is InvocationExpressionSyntax))
                .OfType<InvocationExpressionSyntax>();
            
            foreach (var invocationExpressionSyntax in checkInvocationExpressions)
            {
                if(IsLongMethodChains(context.SemanticModel, invocationExpressionSyntax))
                    ReportDiagnostic(context, invocationExpressionSyntax.GetLocation());
            }
        }
        
        private bool IsLongMethodChains(SemanticModel model, InvocationExpressionSyntax syntax)
        {
            var (fluentInterfaceCallCount, callCount) = Calculate(model, syntax);
            
            var settings = Settings.ChainCallSettings;
            
            if (settings.MaxCallMustIncludeFluentInterfaceCall)
                return (callCount + fluentInterfaceCallCount) > settings.MaxCall;
            
            return callCount > settings.MaxCall || fluentInterfaceCallCount > settings.MaxFluentInterfaceCall;
        }

        private (int FluentInterfaceCallCount, int CallCount) Calculate(SemanticModel model, InvocationExpressionSyntax syntax)
        {
            var returnTypes = CurrentWithChildCall(syntax)
                .Select(e => model.GetSymbolInfo(e).Symbol as IMethodSymbol)
                .Where(e => e != null)
                .Select(e => e.ReturnType)
                .ToList();
            
            var callCount = 1;
            var fluentInterfaceCallCount = 0;
            for (var i = 1; i < returnTypes.Count; i++)
            {
                if (SymbolEqualityComparer.Default.Equals(returnTypes[i], returnTypes[i - 1]))
                {
                    fluentInterfaceCallCount += 1;
                }
                else
                {
                    callCount += 1;
                }
            }

            return (fluentInterfaceCallCount, callCount);
        }
        
        private static IEnumerable<InvocationExpressionSyntax> CurrentWithChildCall(InvocationExpressionSyntax syntax)
        {
            yield return syntax;
            
            foreach (var e in syntax.DescendantNodes(e => e is InvocationExpressionSyntax || e is MemberAccessExpressionSyntax))
            {
                if (e is InvocationExpressionSyntax s)
                    yield return s;
            }
        }
    }
}