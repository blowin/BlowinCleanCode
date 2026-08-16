using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlowinCleanCode.Feature.GoodPractice
{
    public sealed class SwitchNotCoverAllEnumValuesFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase<SwitchStatementSyntax>
    {
        public const string DiagnosticId = "BCC1000";

        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(
            DiagnosticId,
            "Switch does not cover all enum values",
            "Switch does not cover all enum values",
            "Good practice",
            DiagnosticSeverity.Warning,
            "A switch statement on an enum type should handle every enum value or provide a default clause",
            true);

        protected override SyntaxKind SyntaxKind => SyntaxKind.SwitchStatement;

        protected override void Analyze(SyntaxNodeAnalysisContext context, SwitchStatementSyntax syntax)
        {
            if (AnalyzerCommentSkipCheck.Skip(syntax))
                return;

            var enumType = context.SemanticModel.GetTypeInfo(syntax.Expression).Type as INamedTypeSymbol;
            if (enumType == null || enumType.TypeKind != TypeKind.Enum)
                return;

            if (IsFlagsEnum(enumType))
                return;

            if (HasDefaultClause(syntax))
                return;

            var coveredValues = GetCoveredValues(context, syntax);
            foreach (var member in enumType.GetMembers())
            {
                if (!(member is IFieldSymbol field) || !field.IsEnumMember)
                    continue;

                if (field.ConstantValue == null)
                    continue;

                if (TryGetValue(field.ConstantValue, out var memberValue) && coveredValues.Contains(memberValue))
                    continue;

                ReportDiagnostic(context, syntax.GetLocation());
                return;
            }
        }

        private static bool HasDefaultClause(SwitchStatementSyntax syntax)
            => syntax.Sections.SelectMany(s => s.Labels).OfType<DefaultLabelSyntax>().Any();

        private static bool IsFlagsEnum(INamedTypeSymbol enumType)
            => enumType.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == "System.FlagsAttribute");

        private static HashSet<decimal> GetCoveredValues(SyntaxNodeAnalysisContext context, SwitchStatementSyntax syntax)
        {
            var values = new HashSet<decimal>();
            foreach (var section in syntax.Sections)
            {
                foreach (var label in section.Labels)
                {
                    if (label is CaseLabelSyntax caseLabel && caseLabel.Value != null)
                    {
                        var constant = context.SemanticModel.GetConstantValue(caseLabel.Value);
                        if (constant.HasValue && TryGetValue(constant.Value, out var value))
                            values.Add(value);
                    }
                }
            }

            return values;
        }

        private static bool TryGetValue(object value, out decimal result)
        {
            if (value is null)
                return false;

            if (value.GetType().IsEnum)
            {
                value = Convert.ChangeType(value, value.GetType().BaseType, CultureInfo.InvariantCulture);
            }

            switch (value)
            {
                case byte b:
                    result = b;
                    return true;
                case sbyte sb:
                    result = sb;
                    return true;
                case short s:
                    result = s;
                    return true;
                case ushort us:
                    result = us;
                    return true;
                case int i:
                    result = i;
                    return true;
                case uint ui:
                    result = ui;
                    return true;
                case long l:
                    result = (decimal)l;
                    return true;
                case ulong ul:
                    result = (decimal)ul;
                    return true;
                default:
                    return false;
            }
        }
    }
}
