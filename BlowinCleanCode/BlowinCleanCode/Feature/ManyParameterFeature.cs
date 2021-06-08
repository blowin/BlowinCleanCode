using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature
{
    public sealed class ManyParameterFeature : FeatureBase<IMethodSymbol>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.ManyParametersMethod, 
            title: "Method has many parameters",
            messageFormat: "Method '{0}' has many parameters", 
            Constant.Category.SingleResponsibility, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true);

        protected override SymbolKind SymbolKind => SymbolKind.Method;
        
        protected override void Analyze(SymbolAnalysisContext context, IMethodSymbol ms)
        {
            if(ms.Parameters.Length < 4)
                return;

            ReportDiagnostic(context, ms.Locations[0], ms.Name);
        }
    }
}