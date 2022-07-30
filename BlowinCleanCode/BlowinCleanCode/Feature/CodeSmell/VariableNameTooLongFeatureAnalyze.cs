using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.CodeSmell
{
    public sealed class VariableNameTooLongFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.VariableNameTooLong,
            title: "The name is too long.",
            messageFormat: "The name \"{0}\" is too long.",
            Constant.Category.CodeSmell,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override void Register(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(ctx => AnalyzeWithCheck<VariableDeclaratorSyntax>(ctx, Analyze), SyntaxKind.VariableDeclarator);
            context.RegisterSyntaxNodeAction(ctx => AnalyzeWithCheck<ParameterSyntax>(ctx, Analyze), SyntaxKind.Parameter);
            context.RegisterSyntaxNodeAction(ctx => AnalyzeWithCheck<ArgumentSyntax>(ctx, Analyze), SyntaxKind.Argument);
        }

        private void Analyze(SyntaxNodeAnalysisContext context, VariableDeclaratorSyntax variableDeclarationSyntax) => Analyze(context, variableDeclarationSyntax.Identifier);
        private void Analyze(SyntaxNodeAnalysisContext context, ParameterSyntax parameterSyntax) => Analyze(context, parameterSyntax.Identifier);
        private void Analyze(SyntaxNodeAnalysisContext context, ArgumentSyntax argumentSyntax) => AnalyzeIdentifierNameSyntax(context, argumentSyntax.Expression);

        private void AnalyzeIdentifierNameSyntax(SyntaxNodeAnalysisContext context, ExpressionSyntax syntax)
        {
            if(syntax is IdentifierNameSyntax identifierNameSyntax)
                Analyze(context, identifierNameSyntax.Identifier);
        }

        private void Analyze(SyntaxNodeAnalysisContext context, SyntaxToken syntaxNode)
        {
            var variableName = syntaxNode.Text ?? string.Empty;
            if (variableName.Length > Settings.MaxLengthVariableName)
                ReportDiagnostic(context, syntaxNode.GetLocation(), variableName);
        }
    }
}