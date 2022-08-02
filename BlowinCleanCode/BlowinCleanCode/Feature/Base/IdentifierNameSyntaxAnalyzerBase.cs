﻿using System.Linq;
using BlowinCleanCode.Extension.SymbolExtension;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.Base
{
    public abstract class IdentifierNameSyntaxAnalyzerBase : FeatureSyntaxNodeAnalyzerBase
    {
        public override void Register(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(ctx => AnalyzeWithCheck<VariableDeclaratorSyntax>(ctx, Analyze), SyntaxKind.VariableDeclarator);
            context.RegisterSyntaxNodeAction(ctx => AnalyzeWithCheck<ParameterSyntax>(ctx, Analyze), SyntaxKind.Parameter);
            context.RegisterSyntaxNodeAction(ctx => AnalyzeWithCheck<ArgumentSyntax>(ctx, Analyze), SyntaxKind.Argument);
            context.RegisterSyntaxNodeAction(ctx => AnalyzeWithCheck<MemberAccessExpressionSyntax>(ctx, Analyze), SyntaxKind.SimpleMemberAccessExpression);
            context.RegisterSyntaxNodeAction(ctx => AnalyzeWithCheck<TypeDeclarationSyntax>(ctx, Analyze), SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration);
        }

        protected abstract void Analyze(SyntaxNodeAnalysisContext context, SyntaxToken syntaxNode);

        private void Analyze(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDeclarationSyntax)
        {
            Analyze(context, typeDeclarationSyntax.Identifier);
            
            foreach (var memberDeclarationSyntax in typeDeclarationSyntax.Members)
            {
                if (memberDeclarationSyntax is MethodDeclarationSyntax methodDeclarationSyntax)
                {
                    Analyze(context, methodDeclarationSyntax.Identifier);
                }
                else if (memberDeclarationSyntax is PropertyDeclarationSyntax propertyDeclarationSyntax)
                {
                    Analyze(context, propertyDeclarationSyntax.Identifier);
                }
                else if(memberDeclarationSyntax is DelegateDeclarationSyntax delegateDeclarationSyntax)
                {
                    Analyze(context, delegateDeclarationSyntax.Identifier);
                }
                else if (memberDeclarationSyntax is EventDeclarationSyntax eventDeclarationSyntax)
                {
                    Analyze(context, eventDeclarationSyntax.Identifier);
                }
            }
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
    }
}