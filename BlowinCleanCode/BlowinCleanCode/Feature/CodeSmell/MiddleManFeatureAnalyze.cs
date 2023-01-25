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

            var hasAnyMethod = false;
            foreach (var method in namedTypeSymbol.Methods(false).AsSyntax<MethodDeclarationSyntax>())
            {
                hasAnyMethod = true;
                if (!UseOneFieldWithSingleCall(method, fieldSymbol, context))
                    return;
            }

            if(!hasAnyMethod)
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

            var (first, second) = namedTypeSymbol.Fields().Where(e => !e.IsConst).FirstPairOrDefault();
            if (second.HasValue || !first.HasValue)
                return true;

            fieldSymbol = first.Value;
            return false;
        }

        private static bool UseOneFieldWithSingleCall(MethodDeclarationSyntax methodDeclarationSyntax, ISymbol typeField, SyntaxNodeAnalysisContext context)
        {
            var (firstNode, second) = methodDeclarationSyntax.GetBodyChildNodes().FirstPairOrDefault();

            // more than 1 item
            if (second.HasValue)
                return false;

            // No lines
            var expression = ExtractSyntaxNode(firstNode.Value);
            return expression != null && IsAdapterCall(expression, typeField, context);
        }

        private static SyntaxNode ExtractSyntaxNode(SyntaxNode syntaxNode)
        {
            return syntaxNode is ReturnStatementSyntax returnStatementSyntax ? returnStatementSyntax.Expression : syntaxNode;
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
