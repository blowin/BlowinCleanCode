using Microsoft.CodeAnalysis;

namespace BlowinCleanCode.Feature.MethodContain
{
    public sealed class MethodContainAndFeature : MethodContainBaseFeature
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(
            id: Constant.Id.MethodContainAnd,
            title: "Method shouldn't contain 'And'",
            messageFormat: "Method '{0}' contain 'And'",
            Constant.Category.SingleResponsibility, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true
        );

        protected override string CheckContainNameWord => "And";
    }
}