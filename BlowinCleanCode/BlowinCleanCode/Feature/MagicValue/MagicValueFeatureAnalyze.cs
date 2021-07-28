using System.Collections.Generic;
using System.Linq;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.MagicValue
{
    public sealed class MagicValueFeatureSymbolAnalyze : FeatureSyntaxNodeAnalyzerBase<MethodDeclarationSyntax>
    {
        private static readonly MagicValueSkipSyntaxNodeVisitor MagicValueSkipSyntaxNodeVisitor =
            new MagicValueSkipSyntaxNodeVisitor();
        
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
            foreach (var literal in Literals(syntax, context))
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
        
        private IEnumerable<LiteralExpressionSyntax> Literals(MethodDeclarationSyntax syntax, SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            var literalExtractorVisitor = new MagicValueLiteralExtractorVisitor(MagicValueSkipSyntaxNodeVisitor, syntaxNodeContext);
            foreach (var node in syntax.DescendantNodes(n => !MagicValueSkipSyntaxNodeVisitor.Visit(n)))
            {
                if (!(node is CSharpSyntaxNode cSharpSyntaxNode)) 
                    continue;
                
                var literals = cSharpSyntaxNode.Accept(literalExtractorVisitor) ?? Enumerable.Empty<LiteralExpressionSyntax>();
                foreach (var literalExpressionSyntax in literals)
                    yield return literalExpressionSyntax;
            }
        }
    }
}
