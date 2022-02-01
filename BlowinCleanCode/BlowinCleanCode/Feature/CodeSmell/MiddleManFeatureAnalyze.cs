using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Extension.SymbolExtension;
using BlowinCleanCode.Extension.SyntaxExtension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.CodeSmell
{
    public sealed class MiddleManFeatureAnalyze : TypeDeclarationSyntaxNodeAnalyzerBase
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.MiddleMan, 
            title: "Type performs only one action, delegating work to another type",
            messageFormat: "'{0}' performs only one action, delegating work to '{1}'. " +
                           "If your class is an adapter, then specify this at the end of the type.", 
            Constant.Category.CodeSmell, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true);
        
        protected override void Analyze(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax syntaxNode)
        {
            if(!context.ContainingSymbol.Is<INamedTypeSymbol>(out var namedTypeSymbol) || syntaxNode.TypeName().EndsWith("Adapter"))
                return;
            
            var methods = namedTypeSymbol.Methods(false)
                .AsSyntax<MethodDeclarationSyntax>()
                .ToImmutableArray();
            
            if(!AllMethodsUseOneFieldWithSingleCall(namedTypeSymbol, methods, context, out var field))
                return;
            
            ReportDiagnostic(context, syntaxNode.Identifier.GetLocation(), syntaxNode.TypeName(), field.Name);
        }

        private static bool AllMethodsUseOneFieldWithSingleCall(INamedTypeSymbol parentType, 
            ImmutableArray<MethodDeclarationSyntax> methods, 
            SyntaxNodeAnalysisContext context,
            out ISymbol useField)
        {
            useField = default;

            var set = new HashSet<ISymbol>(SymbolEqualityComparer.Default);
            var nodes = new List<SyntaxNode>(2);
            foreach (var methodDeclarationSyntax in methods)
            {
                nodes.Clear();
                nodes.AddRange(methodDeclarationSyntax.GetBodyChildNodes().Take(2));
                if(nodes.Count == 0)
                    continue;

                if (nodes.Count > 1)
                    return false;

                var node = nodes[0];
                if (node is ReturnStatementSyntax returnStatementSyntax)
                    node = returnStatementSyntax.Expression;
                
                if(node == null)
                    continue;

                if (!node.Is<InvocationExpressionSyntax>(out var invocationExpressionSyntax) ||
                    !invocationExpressionSyntax.Expression.Is<MemberAccessExpressionSyntax>(out var memberAccessExpressionSyntax) ||
                    !memberAccessExpressionSyntax.Expression.Is<IdentifierNameSyntax>(out var identifierNameSyntax))
                {
                    return false;
                }

                var field = context.SemanticModel.GetSymbolInfo(identifierNameSyntax, context.CancellationToken).Symbol;
                if (field == null)
                    return false;
                
                // TODO: Check, method doesn't have parameters or all parameters are redirected unchanged
                set.Add(field);
                if (set.Count > 1)
                    return false;
            }

            useField = set.SingleOrDefault();
            if (useField == null)
                return false;

            foreach (var fieldSymbol in parentType.Fields())
            {
                if(fieldSymbol.IsConst)
                    continue;

                if (SymbolEqualityComparer.Default.Equals(fieldSymbol, useField))
                    return true;
            }
            
            return false;
        }
    }
}