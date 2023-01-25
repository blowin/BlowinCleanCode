using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature
{
    public sealed class PublicStaticFieldFeatureSymbolAnalyze : FeatureSymbolAnalyzeBase<IFieldSymbol>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(
            Constant.Id.PublicStaticField,
            title: "Field must be readonly",
            messageFormat: "Field '{0}' mutable",
            Constant.Category.Encapsulation,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Type names should be readonly.");

        protected override SymbolKind SymbolKind => SymbolKind.Field;

        protected override void Analyze(SymbolAnalysisContext context, IFieldSymbol fs)
        {
            if (fs.DeclaredAccessibility != Accessibility.Public)
                return;

            if (fs.IsReadOnly || fs.IsConst)
                return;

            if (!fs.IsStatic)
                return;

            ReportDiagnostic(context, fs.Locations[0], fs.Name);
        }
    }
}
