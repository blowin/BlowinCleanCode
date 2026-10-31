using System;
using BlowinCleanCode.Extension.SyntaxExtension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.CodeSmell
{
    public sealed class NegativeBooleanNamingFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(
            Constant.Id.NegativeBooleanNaming,
            title: "Negative boolean name.",
            messageFormat: "The boolean '{0}' has a negative name; use an affirmative name instead.",
            Constant.Category.CodeSmell,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override void Register(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeField, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeLocalVariable, SyntaxKind.LocalDeclarationStatement);
            context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeParameter, SyntaxKind.Parameter);
        }

        private void AnalyzeField(SyntaxNodeAnalysisContext context)
        {
            AnalyzeWithCheck<FieldDeclarationSyntax>(context, (ctx, field) =>
            {
                foreach (var variable in field.Declaration.Variables)
                    CheckBooleanName(ctx, variable.Identifier, field.Declaration.Type);
            });
        }

        private void AnalyzeLocalVariable(SyntaxNodeAnalysisContext context)
        {
            AnalyzeWithCheck<LocalDeclarationStatementSyntax>(context, (ctx, local) =>
            {
                foreach (var variable in local.Declaration.Variables)
                    CheckBooleanName(ctx, variable.Identifier, local.Declaration.Type);
            });
        }

        private void AnalyzeProperty(SyntaxNodeAnalysisContext context)
        {
            AnalyzeWithCheck<PropertyDeclarationSyntax>(context, (ctx, property) => CheckBooleanName(ctx, property.Identifier, property.Type));
        }

        private void AnalyzeParameter(SyntaxNodeAnalysisContext context)
        {
            AnalyzeWithCheck<ParameterSyntax>(context, (ctx, parameter) => CheckBooleanName(ctx, parameter.Identifier, parameter.Type));
        }

        private void CheckBooleanName(SyntaxNodeAnalysisContext context, SyntaxToken identifier, TypeSyntax type)
        {
            if (!type.IsBool())
                return;

            var settings = Settings.Resolve(context);
            foreach (var word in settings.NegativeBooleanWords)
            {
                if (!HasNegativeWord(identifier.Text, word))
                    continue;
                
                ReportDiagnostic(context, identifier.GetLocation(), identifier.Text);
                return;
            }
        }

        private static bool HasNegativeWord(string name, string word)
        {
            var current = 0;
            for (var i = 0; i <= name.Length; i++)
            {
                if (i == name.Length || name[i] == '_' || (i > 0 && char.IsUpper(name[i]) && char.IsLower(name[i - 1])))
                {
                    var segment = name.AsSpan().Slice(current, i - current);
                    if (segment.Length != 0 && segment.Equals(word.AsSpan(), System.StringComparison.OrdinalIgnoreCase))
                        return true;
                    current = i;
                }
            }
            return false;
        }
    }
}