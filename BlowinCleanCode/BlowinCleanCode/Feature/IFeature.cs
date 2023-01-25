using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature
{
    public interface IFeature
    {
        DiagnosticDescriptor DiagnosticDescriptor { get; }

        void Register(AnalysisContext context);
    }
}
