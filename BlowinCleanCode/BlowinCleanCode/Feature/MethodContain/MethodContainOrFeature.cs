using Microsoft.CodeAnalysis;

namespace BlowinCleanCode.Feature.MethodContain
{
    public sealed class MethodContainOrFeature : MethodContainBaseFeature
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(
            id: Constant.Id.MethodContainOr,
            title: "Method shouldn't contain 'Or'",
            messageFormat: "Method '{0}' contain 'Or'",
            Constant.Category.SingleResponsibility, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true
        );

        protected override string CheckContainNameWord => "Or";
    }
}