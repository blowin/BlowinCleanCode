using System.Collections.Generic;
using System.Linq;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature
{
    public sealed class DeeplyNestedCodeFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase<MethodDeclarationSyntax>
    {
        private static readonly HashSet<SyntaxKind> CheckStatement = new HashSet<SyntaxKind>
        {
            SyntaxKind.IfStatement,
            SyntaxKind.ForStatement,
            SyntaxKind.ForEachKeyword,
            SyntaxKind.ForEachVariableStatement,
            SyntaxKind.DoStatement,
            SyntaxKind.WhileStatement,
            SyntaxKind.TryStatement,
            SyntaxKind.SwitchStatement,
            SyntaxKind.ElseClause,
            SyntaxKind.UsingStatement,
        };
        
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.DeeplyNestedCode, 
            title: "Deeply nested code",
            messageFormat: "Statements should not be nested too deeply", 
            Constant.Category.CodeSmell, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true);

        protected override SyntaxKind SyntaxKind => SyntaxKind.MethodDeclaration;
        
        protected override void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax syntaxNode)
        {
            if(syntaxNode.Body == null)
                return;
            
            foreach (var childNode in syntaxNode.Body.Statements)
            {
                foreach (var descendantNode in AllCheckStatements(childNode))
                {
                    Check(context, descendantNode);
                    if (descendantNode is IfStatementSyntax ifS && ifS.Else?.Statement != null)
                        Check(context, ifS.Else.Statement);
                }   
            }
        }

        private void Check(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            if(AnalyzerCommentSkipCheck.Skip(node))
                return;
            
            var count = Depth(node);
            if (count <= Settings.MaxDeeplyNested) 
                return;
            
            var invalidStatement = node.Ancestors().OfType<StatementSyntax>().FirstOrDefault();
            if(invalidStatement == null)
                return;
            
            ReportDiagnostic(context, invalidStatement.GetLocation());
        }
        
        private static int Depth(SyntaxNode node)
        {
            var max = 0;
            
            foreach (var syntaxNode in node.ChildNodes())
            {
                foreach (var allCheckStatement in AllCheckStatements(syntaxNode))
                {
                    var newDepth = Depth(allCheckStatement);
                    if (newDepth > max)
                        max = newDepth;
                }
            }
            
            return max + 1;
        }

        private static IEnumerable<SyntaxNode> AllCheckStatements(SyntaxNode node)
        {
            return node.DescendantNodes(e => !IsCheckNode(e)).Where(IsCheckNode);
        }

        private static bool IsCheckNode(SyntaxNode node) => node.Kind() == SyntaxKind.Block;
    }
}