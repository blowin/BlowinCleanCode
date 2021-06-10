using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature
{
    public sealed class PublicStaticFieldFeatureSymbolAnalyze : FeatureSymbolAnalyzeBase<IFieldSymbol>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor => Constant.Diagnostic.PublicStaticField;

        protected override SymbolKind SymbolKind => SymbolKind.Field;

        protected override void Analyze(SymbolAnalysisContext context, IFieldSymbol fs)
        {
            if(fs.DeclaredAccessibility != Accessibility.Public)
                return;

            if(fs.IsReadOnly)
                return;

            if(!fs.IsStatic)
                return;

            ReportDiagnostic(context, fs.Locations[0], fs.Name);
        }
    }
}