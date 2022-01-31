using BlowinCleanCode.Extension;
using BlowinCleanCode.Extension.SyntaxExtension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.SingleResponsibility
{
    public sealed class LongMethodFeatureSymbolAnalyze : FeatureSymbolAnalyzeBase<IMethodSymbol>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.LongMethod, 
            title: "Method is long",
            messageFormat: "Method '{0}' too long", 
            Constant.Category.SingleResponsibility, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true, 
            description: "Method must be shorter");

        protected override SymbolKind SymbolKind => SymbolKind.Method;

        protected override void Analyze(SymbolAnalysisContext context, IMethodSymbol ms)
        {
            if(!ms.IsDefinition)
                return;

            var lineOfCode = 0;
            foreach (var reference in ms.DeclaringSyntaxReferences)
            {
                var syntax = reference.GetSyntax(context.CancellationToken) as MethodDeclarationSyntax;
                if(syntax == null)
                    continue;

                lineOfCode += GetLineOfCode(syntax);
            }

            if(lineOfCode <= Settings.MaxCountOfLinesInMethod)
                return;

            ReportDiagnostic(context, ms.Locations[0], ms.Name);
        }

        private static int GetLineOfCode(MethodDeclarationSyntax node)
        {
            if (node.ExpressionBody != null)
                return 1;
            
            if(node.Body != null)
                return node.Body.Statements.CountOfLines();
            
            return 0;
        }
    }
}