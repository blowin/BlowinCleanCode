using System.Collections.Generic;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature
{
    public sealed class MagicValueFeatureSymbolAnalyze : FeatureSyntaxNodeAnalyzerBase<MethodDeclarationSyntax>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.MagicValue,
            title: "Expression shouldn't contain magic value",
            messageFormat: "Magic value '{0}'",
            Constant.Category.CodeSmell, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true
        );

        protected override SyntaxKind SyntaxKind => SyntaxKind.MethodDeclaration;
  
        protected override void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax syntax)
        {
            var semanticModel = context.Compilation.GetSemanticModel(syntax.SyntaxTree);
            var childNode = new ChildNode(syntax, new SkipSyntaxNodeVisitor(syntax, semanticModel));
            foreach (var syntaxNode in childNode.Nodes())
            {
                if (!IsLiteral(syntaxNode) || AnalyzerCommentSkipCheck.Skip(syntaxNode))
                    continue;
                    
                ReportDiagnostic(context, syntaxNode.GetLocation(), syntaxNode.ToFullString());
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
                    if (Skip(childNode))
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
            private readonly SemanticModel _semanticModel;

            public SkipSyntaxNodeVisitor(MethodDeclarationSyntax methodSymbol, SemanticModel semanticModel)
            {
                _methodReturnBool = MethodReturnBool(methodSymbol);
                _semanticModel = semanticModel;
            }
            
            public override bool VisitReturnStatement(ReturnStatementSyntax node)
            {
                if (!_methodReturnBool || !(node.Expression is LiteralExpressionSyntax les)) 
                    return false;
                
                switch (les.Kind())
                {
                    case SyntaxKind.TrueLiteralExpression:
                    case SyntaxKind.FalseLiteralExpression:
                        return true;
                    default:
                        return false;
                }
            }

            public override bool VisitInvocationExpression(InvocationExpressionSyntax node)
            {
                if(node.Expression is MemberAccessExpressionSyntax mas)
                {
                    if (IsFluent(mas))
                        return true;

                    var typeInfo = _semanticModel.GetTypeInfo(mas.Expression);
                    if (typeInfo.Type?.SpecialType == SpecialType.System_String)
                        return true;

                    if (mas.OperatorToken.IsKind(SyntaxKind.DotToken))
                        return mas.Name?.Identifier.Text == "ToString";
                }

                return base.VisitInvocationExpression(node);
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

            private bool IsFluent(MemberAccessExpressionSyntax mas)
            {
                if (!(_semanticModel.GetSymbolInfo(mas.Name).Symbol is IMethodSymbol ms))
                    return false;

                return SymbolEqualityComparer.Default.Equals(ms.ContainingType, ms.ReturnType);
            }
            
            private static bool MethodReturnBool(MethodDeclarationSyntax syntax)
            {
                var kind = syntax.ReturnType.Kind();
                if (syntax.ReturnType is PredefinedTypeSyntax pts)
                    kind = pts.Keyword.Kind();

                return kind == SyntaxKind.BoolKeyword;
            }
        }
    }
}