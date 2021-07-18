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
            foreach (var descendantNode in AllCheckStatements(syntaxNode))
            {
                if(AnalyzerCommentSkipCheck.Skip(descendantNode))
                    continue;
                
                var count = Depth(descendantNode);
                if(count > Settings.MaxDeeplyNested)
                    ReportDiagnostic(context, descendantNode.GetLocation());
            }
        }

        private static int Depth(SyntaxNode node)
        {
            return node
                .ChildNodes()
                .SelectMany(n => AllCheckStatements(n))
                .Select(n => Depth(n))
                .DefaultIfEmpty(0)
                .Max() + 1;;
        }

        private static IEnumerable<SyntaxNode> AllCheckStatements(SyntaxNode node)
        {
            return node.DescendantNodes(e => !IsCheckNode(e)).Where(IsCheckNode);
        }

        private static bool IsCheckNode(SyntaxNode node) => CheckStatement.Contains(node.Kind());
    }
}