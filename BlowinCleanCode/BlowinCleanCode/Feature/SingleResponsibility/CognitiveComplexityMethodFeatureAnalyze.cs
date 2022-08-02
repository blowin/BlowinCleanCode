using BlowinCleanCode.Extension.SyntaxExtension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.SingleResponsibility
{
    public sealed class CognitiveComplexityMethodFeatureAnalyze : FeatureSymbolAnalyzeBase<IMethodSymbol>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.CognitiveComplexity, 
            title: "The method has a coherent cognitive complexity.",
            messageFormat: "The method '{0}' has a coherent cognitive complexity.", 
            Constant.Category.SingleResponsibility, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true, 
            description: "The method should be simpler.");

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

            if(lineOfCode <= Settings.MaxCognitiveComplexity)
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