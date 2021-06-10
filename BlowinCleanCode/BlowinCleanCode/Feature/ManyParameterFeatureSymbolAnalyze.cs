using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature
{
    public sealed class ManyParameterFeatureSymbolAnalyze : FeatureSymbolAnalyzeBase<IMethodSymbol>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor => Constant.Diagnostic.ManyParametersMethod;

        protected override SymbolKind SymbolKind => SymbolKind.Method;
        
        protected override void Analyze(SymbolAnalysisContext context, IMethodSymbol ms)
        {
            if(ms.Parameters.Length < 4)
                return;

            ReportDiagnostic(context, ms.Locations[0], ms.Name);
        }
    }
}