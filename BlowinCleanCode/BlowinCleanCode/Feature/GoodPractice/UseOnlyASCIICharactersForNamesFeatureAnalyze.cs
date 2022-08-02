﻿using BlowinCleanCode.Extension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.GoodPractice
{
    public sealed class UseOnlyASCIICharactersForNamesFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.UseOnlyASCIICharactersForNames,
            title: "Use only ascii symbols in identifiers.",
            messageFormat: "The name \"{0}\" contains non-ascii characters.",
            Constant.Category.GoodPractice,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override void Register(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(ctx => AnalyzeWithCheck<VariableDeclaratorSyntax>(ctx, Analyze), SyntaxKind.VariableDeclarator);
            context.RegisterSyntaxNodeAction(ctx => AnalyzeWithCheck<ParameterSyntax>(ctx, Analyze), SyntaxKind.Parameter);
            context.RegisterSyntaxNodeAction(ctx => AnalyzeWithCheck<ArgumentSyntax>(ctx, Analyze), SyntaxKind.Argument);
            context.RegisterSyntaxNodeAction(ctx => AnalyzeWithCheck<MemberAccessExpressionSyntax>(ctx, Analyze), SyntaxKind.SimpleMemberAccessExpression);
        }

        private void Analyze(SyntaxNodeAnalysisContext context, VariableDeclaratorSyntax variableDeclarationSyntax) => Analyze(context, variableDeclarationSyntax.Identifier);
        private void Analyze(SyntaxNodeAnalysisContext context, ParameterSyntax parameterSyntax) => Analyze(context, parameterSyntax.Identifier);
        private void Analyze(SyntaxNodeAnalysisContext context, ArgumentSyntax argumentSyntax) => AnalyzeIdentifierNameSyntax(context, argumentSyntax.Expression);
        private void Analyze(SyntaxNodeAnalysisContext context, MemberAccessExpressionSyntax memberAccessExpression) => AnalyzeIdentifierNameSyntax(context, memberAccessExpression.Expression);

        private void AnalyzeIdentifierNameSyntax(SyntaxNodeAnalysisContext context, ExpressionSyntax syntax)
        {
            foreach (var childNode in syntax.DescendantNodesAndSelf(v => !AnalyzerCommentSkipCheck.Skip(v)))
            {
                if (AnalyzerCommentSkipCheck.Skip(childNode))
                    continue;

                if (childNode is IdentifierNameSyntax identifierNameSyntax)
                    Analyze(context, identifierNameSyntax.Identifier);
            }
        }

        private void Analyze(SyntaxNodeAnalysisContext context, SyntaxToken syntaxNode)
        {
            var variableName = syntaxNode.Text ?? string.Empty;
            if (!variableName.IsAscii())
                ReportDiagnostic(context, syntaxNode.GetLocation(), variableName);
        }
    }
}