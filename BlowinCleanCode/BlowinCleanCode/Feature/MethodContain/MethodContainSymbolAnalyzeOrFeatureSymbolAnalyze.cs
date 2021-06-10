using Microsoft.CodeAnalysis;

namespace BlowinCleanCode.Feature.MethodContain
{
    public sealed class MethodContainSymbolAnalyzeOrFeatureSymbolAnalyze : MethodContainSymbolAnalyzeBaseFeatureSymbolAnalyze
    {
        public override DiagnosticDescriptor DiagnosticDescriptor => Constant.Diagnostic.MethodContainOr;

        protected override string CheckContainNameWord => "Or";
    }
}