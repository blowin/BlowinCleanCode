using System.Collections.Generic;
using System.Linq;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature
{
    public sealed class MagicValueFeatureSymbolAnalyze : FeatureSyntaxNodeAnalyzerBase<MethodDeclarationSyntax>
    {
        private static readonly List<string> SkipLiteralValues = new List<string>
        {
            "0",
            "1",
            "-1",
        };
        
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(
            Constant.Id.MagicValue,
            title: "Expression shouldn't contain magic value",
            messageFormat: "Magic value '{0}'",
            Constant.Category.CodeSmell,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        protected override SyntaxKind SyntaxKind => SyntaxKind.MethodDeclaration;

        protected override void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax syntax)
        {
            var visitor = new SkipSyntaxNodeVisitor(context.SemanticModel);
            foreach (var literal in Literals(syntax, visitor))
            {
                if (AnalyzerCommentSkipCheck.Skip(literal))
                    continue;

                if (literal.IsKind(SyntaxKind.NullLiteralExpression))
                    continue;

                if (SkipLiteralValues.Contains(literal.Token.ValueText ?? string.Empty))
                    continue;
                
                ReportDiagnostic(context, literal.GetLocation(), literal.ToFullString());
            }
        }
        
        private IEnumerable<LiteralExpressionSyntax> Literals(MethodDeclarationSyntax syntax, SkipSyntaxNodeVisitor skipVisitor)
        {
            var literalExtractorVisitor = new LiteralExtractorVisitor(syntax, skipVisitor);
            foreach (var node in syntax.DescendantNodes(n => !Skip(n, skipVisitor)))
            {
                if (node is CSharpSyntaxNode cSharpSyntaxNode)
                {
                    var literals = cSharpSyntaxNode.Accept(literalExtractorVisitor) ?? Enumerable.Empty<LiteralExpressionSyntax>();
                    foreach (var literalExpressionSyntax in literals)
                        yield return literalExpressionSyntax;
                }
            }
        }
        
        private bool Skip(SyntaxNode node, SkipSyntaxNodeVisitor skipVisitor)
        {
            if (!(node is CSharpSyntaxNode csn))
                return true;

            return csn.Accept(skipVisitor);
        }
       
        private static bool MethodReturnBool(MethodDeclarationSyntax syntax)
        {
            var kind = syntax.ReturnType.Kind();
            if (syntax.ReturnType is PredefinedTypeSyntax pts)
                kind = pts.Keyword.Kind();

            return kind == SyntaxKind.BoolKeyword;
        }
        
        private sealed class SkipSyntaxNodeVisitor : CSharpSyntaxVisitor<bool>
        {
            private readonly SemanticModel _semanticModel;

            public SkipSyntaxNodeVisitor(SemanticModel semanticModel)
            {
                _semanticModel = semanticModel;
            }

            public override bool VisitInvocationExpression(InvocationExpressionSyntax node)
            {
                if (node.Expression is MemberAccessExpressionSyntax mas)
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

            public override bool VisitReturnStatement(ReturnStatementSyntax node) => true;

            public override bool VisitElementAccessExpression(ElementAccessExpressionSyntax node) => true;

            public override bool VisitAssignmentExpression(AssignmentExpressionSyntax node) => true;

            public override bool VisitArgument(ArgumentSyntax node)
            {
                if (node.Expression is InvocationExpressionSyntax)
                    return false;
                
                if (node.NameColon != null)
                    return true;
                
                // Method <- ( <- 1);
                // argument -> argumentList -> invocation
                if (node.Parent?.Parent is InvocationExpressionSyntax && _semanticModel.GetSymbolInfo(node.Parent.Parent).Symbol is IMethodSymbol methodSymbol)
                {
                    var parameterCount = methodSymbol.Parameters.Length;
                    if (parameterCount <= 2 && methodSymbol.IsExtensionMethod)
                        return true;

                    if (parameterCount <= 1)
                        return true;
                }
                
                return false;
            }

            public override bool VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node) =>
                node.IsConst;

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
        }
        
        private sealed class LiteralExtractorVisitor : CSharpSyntaxVisitor<IEnumerable<LiteralExpressionSyntax>>
        {
            private readonly bool _methodReturnBool;
            private readonly bool _methodReturnTuple;
            private readonly SkipSyntaxNodeVisitor _skipVisitor;

            public LiteralExtractorVisitor(MethodDeclarationSyntax syntax, SkipSyntaxNodeVisitor skipVisitor)
            {
                _skipVisitor = skipVisitor;
                _methodReturnBool = MethodReturnBool(syntax);
                _methodReturnTuple = syntax.ReturnType is TupleTypeSyntax;
            }

            public override IEnumerable<LiteralExpressionSyntax> VisitLiteralExpression(LiteralExpressionSyntax node)
            {
                return node.ToSingleEnumerable();
            }

            public override IEnumerable<LiteralExpressionSyntax> VisitReturnStatement(ReturnStatementSyntax node)
            {
                if (!_methodReturnBool && !_methodReturnTuple)
                {
                    var invalidItems = node
                        .ChildNodes()
                        .SelectMany(e => e.DescendantNodes(n => !Skip(n)).OfType<LiteralExpressionSyntax>());
                            
                    foreach (var literalExpressionSyntax in invalidItems)
                        yield return literalExpressionSyntax;
                }
                else
                {
                    foreach (var returnInvalidLiteralNode in GetReturnInvalidLiteralNodes(node, false))
                    {
                        if (returnInvalidLiteralNode is LiteralExpressionSyntax rl)
                            yield return rl;
                    }   
                }
            }

            private IEnumerable<LiteralExpressionSyntax> GetReturnInvalidLiteralNodes(SyntaxNode parent, bool canBeInvalid)
            {
                foreach (var syntaxNode in parent.ChildNodes())
                {
                    if(Skip(syntaxNode))
                        continue;

                    if (syntaxNode is TupleExpressionSyntax && !canBeInvalid)
                        continue;

                    if (syntaxNode is LiteralExpressionSyntax literalExpressionSyntax)
                    {
                        if (canBeInvalid)
                            yield return literalExpressionSyntax;
                    }
                    else
                    {
                        foreach (var returnInvalidLiteralNode in GetReturnInvalidLiteralNodes(syntaxNode, !(syntaxNode is ConditionalExpressionSyntax)))
                            yield return returnInvalidLiteralNode;
                    }
                }
            }
            
            private bool Skip(SyntaxNode node)
            {
                if (!(node is CSharpSyntaxNode csn))
                    return true;

                return csn.Accept(_skipVisitor);
            }
        }
    }
}
