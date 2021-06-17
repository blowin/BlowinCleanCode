using System.Collections.Generic;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature
{
    public sealed class MagicValueFeatureSymbolAnalyze : FeatureSymbolAnalyzeBase<IMethodSymbol>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.MagicValue,
            title: "Expression shouldn't contain magic value",
            messageFormat: "Magic value '{0}'",
            Constant.Category.SingleResponsibility, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true
        );

        protected override SymbolKind SymbolKind => SymbolKind.Method;
        
        protected override void Analyze(SymbolAnalysisContext context, IMethodSymbol symbol)
        {
            foreach (var reference in symbol.DeclaringSyntaxReferences)
            {
                if(!(reference.GetSyntax(context.CancellationToken) is MethodDeclarationSyntax syntax))
                    continue;

                var childNode = new ChildNode(syntax, new SkipSyntaxNodeVisitor(syntax));
                foreach (var syntaxNode in childNode.Nodes())
                {
                    if (!IsLiteral(syntaxNode) || AnalyzerCommentSkipCheck.Skip(syntaxNode))
                        continue;
                    
                    ReportDiagnostic(context, syntaxNode.GetLocation(), syntaxNode.ToFullString());
                }
            }
        }
        
        private static bool IsLiteral(SyntaxNode node) => node is LiteralExpressionSyntax && !node.IsKind(SyntaxKind.NullLiteralExpression);

        private readonly struct ChildNode
        {
            private readonly SkipSyntaxNodeVisitor _skipCheck;
            private readonly MethodDeclarationSyntax _syntax;
            
            public ChildNode(MethodDeclarationSyntax syntax, SkipSyntaxNodeVisitor skipSyntax)
            {
                _syntax = syntax;
                _skipCheck = skipSyntax;
            }
            
            public IEnumerable<SyntaxNode> Nodes()
            {
                if (_syntax.Body != null)
                {
                    foreach (var statementSyntax in _syntax.Body.Statements)
                    {
                        foreach (var syntaxNode in AllChild(statementSyntax, true))
                            yield return syntaxNode;
                    }
                }
                else if (_syntax.ExpressionBody != null)
                {
                    foreach (var syntaxNode in AllChild(_syntax.ExpressionBody,  true))
                        yield return syntaxNode;
                }
            }

            private IEnumerable<SyntaxNode> AllChild(SyntaxNode node, bool checkKind)
            {
                if (Skip(node))
                    yield break;
                
                foreach (var childNode in node.ChildNodes())
                {
                    if (Skip(node))
                        continue;

                    if (!checkKind || VisitKind(childNode))
                    {
                        yield return childNode;

                        foreach (var n2 in AllChild(childNode, false))
                            yield return n2;
                    }
                }
            }

            private bool Skip(SyntaxNode node)
            {
                if (!(node is CSharpSyntaxNode csn))
                    return true;

                return csn.Accept(_skipCheck);
            }

            private static bool VisitKind(SyntaxNode node)
            {
                switch (node.Kind())
                {
                    case SyntaxKind.InvocationExpression:
                    case SyntaxKind.SubtractExpression:
                    case SyntaxKind.AddExpression:
                    case SyntaxKind.MultiplyExpression:
                    case SyntaxKind.DivideExpression:
                    case SyntaxKind.ReturnStatement:
                    case SyntaxKind.ReturnKeyword:
                    case SyntaxKind.ArgumentList:
                    case SyntaxKind.VariableDeclaration:
                    case SyntaxKind.DeclarationExpression:
                        return true;
                    default:
                        return false;
                }
            }
        }
   
        private sealed class SkipSyntaxNodeVisitor : CSharpSyntaxVisitor<bool>
        {
            private readonly bool _methodReturnBool;

            public SkipSyntaxNodeVisitor(MethodDeclarationSyntax methodSymbol)
            {
                _methodReturnBool = methodSymbol.ReturnType.IsKind(SyntaxKind.BoolKeyword);
            }

            public override bool VisitReturnStatement(ReturnStatementSyntax node)
            {
                return _methodReturnBool;
            }

            public override bool VisitElementAccessExpression(ElementAccessExpressionSyntax node) => true;

            public override bool VisitArgument(ArgumentSyntax node) => node.NameColon != null;

            public override bool VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node) => node.IsConst;

            public override bool VisitVariableDeclaration(VariableDeclarationSyntax node)
            {
                foreach (var variableDeclaratorSyntax in node.Variables)
                {
                    var initializeValue = variableDeclaratorSyntax.Initializer.Value;
                    if (initializeValue.IsKind(SyntaxKind.InvocationExpression))
                        return false;
                }

                return true;
            }
        }
    }
}