using Microsoft.CodeAnalysis;

namespace BlowinCleanCode.Feature.MethodContain
{
    public sealed class MethodContainSymbolAnalyzeAndFeatureSymbolAnalyze : MethodContainSymbolAnalyzeBaseFeatureSymbolAnalyze
    {
        public override DiagnosticDescriptor DiagnosticDescriptor => Constant.Diagnostic.MethodContainAnd;

        protected override string CheckContainNameWord => "And";
    }
}