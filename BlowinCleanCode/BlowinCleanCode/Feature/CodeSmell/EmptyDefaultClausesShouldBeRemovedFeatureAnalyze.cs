using BlowinCleanCode.Extension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.CodeSmell
{
    public sealed class EmptyDefaultClausesShouldBeRemovedFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase<DefaultSwitchLabelSyntax>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.EmptyDefaultClausesShouldBeRemoved, 
            title: "Empty 'default' clauses should be removed",
            messageFormat: "The 'default' clause should take appropriate action. Having an empty 'default' is a waste of keystrokes.", 
            Constant.Category.CodeSmell, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true);
        
        protected override SyntaxKind SyntaxKind => SyntaxKind.DefaultSwitchLabel;
        
        protected override void Analyze(SyntaxNodeAnalysisContext context, DefaultSwitchLabelSyntax syntaxNode)
        {
            if(!syntaxNode.Parent.Is<SwitchSectionSyntax>(out var selectionSyntax))
                return;
            
            if(AnalyzerCommentSkipCheck.Skip(selectionSyntax))
                return;
            
            if(!TryGetSingleBreak(selectionSyntax.Statements, out var breakStatement))
                return;
            
            ReportDiagnostic(context, breakStatement.GetLocation());
        }

        private bool TryGetSingleBreak(SyntaxList<StatementSyntax> statements, out BreakStatementSyntax breakStatement)
        {
            breakStatement = default;
            
            var deepLevel = 0;
            const int maxDeepLevel = 256;
            while (true)
            {
                deepLevel += 1;
                if (deepLevel > maxDeepLevel)
                    return false;
                
                if (statements.Count != 1) 
                    return false;

                var statement = statements.First();
                breakStatement = statement as BreakStatementSyntax;
                if (breakStatement == null && statement.Is<BlockSyntax>(out var blockSyntax))
                {
                    statements = blockSyntax.Statements;
                    continue;
                }

                return breakStatement != null;
            }
        }
    }
}