﻿using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.CodeSmell
{
    public sealed class HollowTypeNameFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.HollowTypeName, 
            title: "Hollow type name",
            messageFormat: "'{0}' has a name that doesn't express its intent.", 
            Constant.Category.CodeSmell, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true);

        public override void Register(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(ctx => AnalyzeWithCheck<ClassDeclarationSyntax>(ctx, Analyze), SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(ctx => AnalyzeWithCheck<StructDeclarationSyntax>(ctx, Analyze), SyntaxKind.StructDeclaration);
        }

        private void Analyze(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax syntaxNode)
        {
            var identifier = syntaxNode.Identifier;
            var name = identifier.Text ?? string.Empty;
            foreach (var (word, validateWhenFullMatch) in Settings.HollowTypeNameDictionary)
            {
                if(!validateWhenFullMatch && name.Equals(word))
                    continue;

                if (!name.EndsWith(word)) 
                    continue;
                
                if(AnalyzerCommentSkipCheck.Skip(syntaxNode))
                    continue;
                
                ReportDiagnostic(context, identifier.GetLocation(), name);
                return;
            }
        }
    }
}