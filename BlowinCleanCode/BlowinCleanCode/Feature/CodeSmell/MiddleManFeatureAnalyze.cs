using System.Collections.Immutable;
using System.Linq;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Extension.SymbolExtension;
using BlowinCleanCode.Extension.SyntaxExtension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.CodeSmell
{
    public sealed class MiddleManFeatureAnalyze : TypeDeclarationSyntaxNodeAnalyzerBase
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(
            Constant.Id.MiddleMan,
            title: "Type performs only one action, delegating work to another type",
            messageFormat: "'{0}' performs only one action, delegating work to '{1}'. If your class is an adapter, then specify this at the end of the type.",
            Constant.Category.CodeSmell,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        protected override void Analyze(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax syntaxNode)
        {
            if (Skip(context, syntaxNode, out var namedTypeSymbol, out var fieldSymbol))
                return;

            var methods = namedTypeSymbol.Methods(false)
                .AsSyntax<MethodDeclarationSyntax>()
                .ToImmutableArray();

            if (!AllMethodsUseOneFieldWithSingleCall(methods, fieldSymbol, context))
                return;

            ReportDiagnostic(context, syntaxNode.Identifier.GetLocation(), syntaxNode.TypeName(), fieldSymbol.Name);
        }

        private static bool Skip(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax syntaxNode, out INamedTypeSymbol namedTypeSymbol, out IFieldSymbol fieldSymbol)
        {
            fieldSymbol = default;
            if (!context.ContainingSymbol.Is(out namedTypeSymbol) || syntaxNode.TypeName().EndsWith("Adapter"))
                return true;

            if (namedTypeSymbol.IsStatic)
                return true;

            var pair = namedTypeSymbol.Fields().Where(e => !e.IsConst).FirstPairOrDefault();
            if (pair == null)
                return true;

            var (first, second) = pair.Value;
            if (second != null)
                return true;

            fieldSymbol = first;
            return false;
        }

        // TODO: fix
#pragma warning disable BCC3007 // The name is too long.
#pragma warning disable BCC2000 // The method has a coherent cognitive complexity.
        private static bool AllMethodsUseOneFieldWithSingleCall(ImmutableArray<MethodDeclarationSyntax> methods, ISymbol typeField, SyntaxNodeAnalysisContext context)
#pragma warning restore BCC2000 // The method has a coherent cognitive complexity.
#pragma warning restore BCC3007 // The name is too long.
        {
            var hasAnyCall = false;

            foreach (var methodDeclarationSyntax in methods)
            {
                var firstPairOrDefault = methodDeclarationSyntax.GetBodyChildNodes().FirstPairOrDefault();
                if (firstPairOrDefault == null)
                    continue;

                var (firstNode, second) = firstPairOrDefault.Value;

                // more than 2 item
                if (second != null)
                    return false;

                if (firstNode is ReturnStatementSyntax returnStatementSyntax)
                    firstNode = returnStatementSyntax.Expression;

                if (firstNode == null)
                    continue;

                if (!IsAdapterCall(firstNode, typeField, context))
                    return false;

                hasAnyCall = true;
            }

            return hasAnyCall;
        }

        private static bool IsAdapterCall(SyntaxNode node, ISymbol typeField, SyntaxNodeAnalysisContext context)
        {
            // if not simple call, like this: obj.Run();
            if (!node.Is<InvocationExpressionSyntax>(out var invocationExpressionSyntax) ||
                !invocationExpressionSyntax.Expression.Is<MemberAccessExpressionSyntax>(out var memberAccessExpression) ||
                !memberAccessExpression.Expression.Is<IdentifierNameSyntax>(out var identifierNameSyntax))
            {
                return false;
            }

            var field = context.SemanticModel.GetSymbolInfo(identifierNameSyntax, context.CancellationToken).Symbol;
            if (field == null)
                return false;

            return SymbolEqualityComparer.Default.Equals(field, typeField);
        }
    }
}
