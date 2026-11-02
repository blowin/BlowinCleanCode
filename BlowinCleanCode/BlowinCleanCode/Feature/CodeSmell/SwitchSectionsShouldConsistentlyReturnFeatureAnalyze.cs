using System.Linq;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.CodeSmell
{
    public sealed class SwitchSectionsShouldConsistentlyReturnFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase<SwitchStatementSyntax>
    {
        private static readonly TerminatingStatementVisitor TerminatingStatementVisitorInstance = new TerminatingStatementVisitor();
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(
            Constant.Id.SwitchSectionsShouldConsistentlyReturn,
            title: "Switch sections should consistently return or throw",
            messageFormat: "Switch sections should consistently return or throw",
            Constant.Category.CodeSmell,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        protected override SyntaxKind SyntaxKind => SyntaxKind.SwitchStatement;

        protected override void Analyze(SyntaxNodeAnalysisContext context, SwitchStatementSyntax syntaxNode)
        {
            if (!syntaxNode.Sections.Any(ReturnFinder.ContainsReturn))
                return;

            if (syntaxNode.Sections.All(section => section.Statements.All(TerminatingStatementVisitorInstance.Visit)))
                return;

            ReportDiagnostic(context, syntaxNode.SwitchKeyword.GetLocation());
        }

        private sealed class ReturnFinder : CSharpSyntaxWalker
        {
            private bool HasReturn { get; set; }

            public static bool ContainsReturn(SwitchSectionSyntax section)
            {
                var finder = new ReturnFinder();
                finder.Visit(section);
                return finder.HasReturn;
            }

            public override void VisitReturnStatement(ReturnStatementSyntax node) => HasReturn = true;
        }

        private sealed class TerminatingStatementVisitor : CSharpSyntaxVisitor<bool>
        {
            public override bool DefaultVisit(SyntaxNode node) => false;

            public override bool VisitReturnStatement(ReturnStatementSyntax node) => true;

            public override bool VisitThrowStatement(ThrowStatementSyntax node) => true;

            public override bool VisitBlock(BlockSyntax node) => Terminates(node.Statements);

            public override bool VisitIfStatement(IfStatementSyntax node)
            {
                return node.Else != null
                       && Visit(node.Statement)
                       && Visit(node.Else.Statement);
            }

            public override bool VisitSwitchStatement(SwitchStatementSyntax node)
            {
                return node.Sections.Any(section => section.Labels.Any(label => label.Kind() == SyntaxKind.DefaultSwitchLabel))
                       && node.Sections.All(section => Terminates(section.Statements));
            }

            public override bool VisitTryStatement(TryStatementSyntax node)
            {
                if (node.Finally != null && Visit(node.Finally.Block))
                    return true;

                return Terminates(node.Block.Statements)
                       && node.Catches.All(catchClause => Terminates(catchClause.Block.Statements));
            }

            public override bool VisitLabeledStatement(LabeledStatementSyntax node) => Visit(node.Statement);

            public override bool VisitUsingStatement(UsingStatementSyntax node) => Visit(node.Statement);

            public override bool VisitLockStatement(LockStatementSyntax node) => Visit(node.Statement);

            public override bool VisitFixedStatement(FixedStatementSyntax node) => Visit(node.Statement);

            public override bool VisitCheckedStatement(CheckedStatementSyntax node) => Visit(node.Block);

            public override bool VisitUnsafeStatement(UnsafeStatementSyntax node) => Visit(node.Block);

            private bool Terminates(SyntaxList<StatementSyntax> statements)
            {
                foreach (var statement in statements)
                {
                    if (Visit(statement))
                        return true;
                }

                return false;
            }
        }
    }
}
