using System.Collections.Generic;
using System.Linq;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Feature.Base;
using BlowinCleanCode.Model.Settings;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.SingleResponsibility
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
            var settings = Settings.Resolve(context).ChainCallSettings;

            var checkInvocationExpressions = syntaxNode
                // Don't check child calls
                .DescendantNodes(i => !i.Is<InvocationExpressionSyntax>())
                .OfType<InvocationExpressionSyntax>();
            
            foreach (var invocationExpressionSyntax in checkInvocationExpressions)
            {
                if(AnalyzerCommentSkipCheck.Skip(invocationExpressionSyntax))
                    continue;
                
                if(IsLongMethodChains(context.SemanticModel, invocationExpressionSyntax, settings))
                    ReportDiagnostic(context, invocationExpressionSyntax.GetLocation());
            }
        }
        
        private static bool IsLongMethodChains(SemanticModel model, InvocationExpressionSyntax syntax, AnalyzerChainCallSettings settings)
        {
            var (fluentInterfaceCallCount, callCount) = Calculate(model, syntax);
            
            if (settings.MaxCallMustIncludeFluentInterfaceCall)
                return (callCount + fluentInterfaceCallCount) > settings.MaxCall;
            
            return callCount > settings.MaxCall || fluentInterfaceCallCount > settings.MaxFluentInterfaceCall;
        }

        private static (int FluentInterfaceCallCount, int CallCount) Calculate(SemanticModel model, InvocationExpressionSyntax syntax)
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
            
            foreach (var e in syntax.DescendantNodes(e => e.IsAny<InvocationExpressionSyntax, MemberAccessExpressionSyntax>()))
            {
                if (e is InvocationExpressionSyntax s)
                    yield return s;
            }
        }
    }
}