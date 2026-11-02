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
            var caseExpressions = new List<ExpressionSyntax>();
            foreach (var section in syntaxNode.Sections)
            {
                foreach (var label in section.Labels)
                {
                    AddCaseExpressions(label, caseExpressions);
                }
            }

            AnalyzeSwitch(context, syntaxNode.Expression, syntaxNode.SwitchKeyword, caseExpressions);
        }

        private void AnalyzeExpression(SyntaxNodeAnalysisContext context, SwitchExpressionSyntax syntaxNode)
        {
            var caseExpressions = new List<ExpressionSyntax>();
            foreach (var arm in syntaxNode.Arms)
            {
                AddConstantExpressions(arm.Pattern, caseExpressions);
            }

            AnalyzeSwitch(context, syntaxNode.GoverningExpression, syntaxNode.SwitchKeyword, caseExpressions);
        }

        private void AnalyzeSwitch(
            SyntaxNodeAnalysisContext context, 
            ExpressionSyntax governingExpression, 
            SyntaxToken switchKeyword, 
            List<ExpressionSyntax> caseExpressions)
        {
            if (!(context.SemanticModel.GetTypeInfo(governingExpression).Type is INamedTypeSymbol enumType) ||
                enumType.TypeKind != TypeKind.Enum ||
                IsFlagsEnum(context.Compilation, enumType))
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
            {
                return;
            }

            var coveredValues = new HashSet<object>();
            foreach (var expression in caseExpressions)
            {
                var constant = context.SemanticModel.GetConstantValue(expression);
                if (constant.HasValue && constant.Value != null)
                {
                    coveredValues.Add(constant.Value);
                }
            }

            List<string> missingMembers = null;
            foreach (var field in enumMembers)
            {
                if (coveredValues.Contains(field.ConstantValue))
                    continue;

                if (missingMembers == null)
                    missingMembers = new List<string>();

                missingMembers.Add(field.Name);
            }

            if (missingMembers != null)
            {
                ReportDiagnostic(context, switchKeyword.GetLocation(), string.Join(", ", missingMembers));
            }
        }

        private static bool IsFlagsEnum(Compilation compilation, INamedTypeSymbol enumType)
        {
            var flagsAttributeSymbol = compilation.GetTypeByMetadataName("System.FlagsAttribute");
            if (flagsAttributeSymbol == null) return false;

            foreach (var attribute in enumType.GetAttributes())
            {
                if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, flagsAttributeSymbol))
                {
                    return true;
                }
            }
            return false;
        }

        private static void AddConstantExpressions(PatternSyntax pattern, List<ExpressionSyntax> results)
        {
            switch (pattern)
            {
                case ConstantPatternSyntax constantPattern:
                    results.Add(constantPattern.Expression);
                    break;
                case ParenthesizedPatternSyntax parenthesizedPattern:
                    AddConstantExpressions(parenthesizedPattern.Pattern, results);
                    break;
                case BinaryPatternSyntax binaryPattern when binaryPattern.IsKind(SyntaxKind.OrPattern):
                    AddConstantExpressions(binaryPattern.Left, results);
                    AddConstantExpressions(binaryPattern.Right, results);
                    break;
            }
        }

        private static void AddCaseExpressions(SwitchLabelSyntax label, List<ExpressionSyntax> results)
        {
            if (label is CaseSwitchLabelSyntax caseLabel)
            {
                results.Add(caseLabel.Value);
            }
            else if (label is CasePatternSwitchLabelSyntax patternLabel)
            {
                AddConstantExpressions(patternLabel.Pattern, results);
            }
        }
    }
}
