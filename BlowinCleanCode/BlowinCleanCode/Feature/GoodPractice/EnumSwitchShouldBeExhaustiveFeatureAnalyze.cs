using System;
using System.Collections.Generic;
using System.Linq;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.GoodPractice
{
    public sealed class EnumSwitchShouldBeExhaustiveFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(
            Constant.Id.EnumSwitchShouldBeExhaustive,
            title: "Enum switches should handle every declared value",
            messageFormat: "Switch is missing enum member(s): {0}",
            Constant.Category.GoodPractice,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override void Register(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(
                analysisContext => AnalyzeWithCheck<SwitchStatementSyntax>(analysisContext, AnalyzeStatement),
                SyntaxKind.SwitchStatement);
            context.RegisterSyntaxNodeAction(
                analysisContext => AnalyzeWithCheck<SwitchExpressionSyntax>(analysisContext, AnalyzeExpression),
                SyntaxKind.SwitchExpression);
        }

        private void AnalyzeStatement(SyntaxNodeAnalysisContext context, SwitchStatementSyntax syntaxNode)
        {
            var caseExpressions = syntaxNode.Sections
                .SelectMany(section => section.Labels)
                .SelectMany(CaseExpressions);

            AnalyzeSwitch(context, syntaxNode.Expression, syntaxNode.SwitchKeyword, caseExpressions);
        }

        private void AnalyzeExpression(SyntaxNodeAnalysisContext context, SwitchExpressionSyntax syntaxNode)
        {
            var caseExpressions = syntaxNode.Arms
                .SelectMany(arm => ConstantExpressions(arm.Pattern));

            AnalyzeSwitch(context, syntaxNode.GoverningExpression, syntaxNode.SwitchKeyword, caseExpressions);
        }

        private void AnalyzeSwitch(
            SyntaxNodeAnalysisContext context,
            ExpressionSyntax governingExpression,
            SyntaxToken switchKeyword,
            IEnumerable<ExpressionSyntax> caseExpressions)
        {
            if (!(context.SemanticModel.GetTypeInfo(governingExpression).Type is INamedTypeSymbol enumType)
                || enumType.TypeKind != TypeKind.Enum
                || IsFlagsEnum(enumType))
            {
                return;
            }

            var declaredValues = new HashSet<object>();
            var enumMembers = new List<IFieldSymbol>();
            foreach (var field in enumType.GetMembers().OfType<IFieldSymbol>())
            {
                if (!field.HasConstantValue || field.ConstantValue == null || !declaredValues.Add(field.ConstantValue))
                    continue;

                enumMembers.Add(field);
            }

            if (enumMembers.Count == 0)
                return;

            var coveredValues = new HashSet<object>();
            foreach (var expression in caseExpressions)
            {
                var constant = context.SemanticModel.GetConstantValue(expression);
                if (constant.HasValue && constant.Value != null)
                    coveredValues.Add(constant.Value);
            }

            var missingMembers = enumMembers
                .Where(field => !coveredValues.Contains(field.ConstantValue))
                .Select(field => field.Name)
                .ToArray();

            if (missingMembers.Length > 0)
                ReportDiagnostic(context, switchKeyword.GetLocation(), string.Join(", ", missingMembers));
        }

        private static bool IsFlagsEnum(INamedTypeSymbol enumType)
        {
            return enumType.GetAttributes().Any(attribute =>
                attribute.AttributeClass?.MetadataName == "FlagsAttribute"
                && attribute.AttributeClass.ContainingNamespace.ToDisplayString() == "System");
        }

        private static IEnumerable<ExpressionSyntax> ConstantExpressions(PatternSyntax pattern)
        {
            if (pattern is ConstantPatternSyntax constantPattern)
            {
                yield return constantPattern.Expression;
            }
            else if (pattern is ParenthesizedPatternSyntax parenthesizedPattern)
            {
                foreach (var expression in ConstantExpressions(parenthesizedPattern.Pattern))
                    yield return expression;
            }
            else if (pattern is BinaryPatternSyntax binaryPattern
                     && binaryPattern.IsKind(SyntaxKind.OrPattern))
            {
                foreach (var expression in ConstantExpressions(binaryPattern.Left))
                    yield return expression;
                foreach (var expression in ConstantExpressions(binaryPattern.Right))
                    yield return expression;
            }
        }

        private static IEnumerable<ExpressionSyntax> CaseExpressions(SwitchLabelSyntax label)
        {
            if (label is CaseSwitchLabelSyntax caseLabel)
            {
                yield return caseLabel.Value;
            }
            else if (label is CasePatternSwitchLabelSyntax patternLabel)
            {
                foreach (var expression in ConstantExpressions(patternLabel.Pattern))
                    yield return expression;
            }
        }
    }
}
