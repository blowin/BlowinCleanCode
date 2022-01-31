using BlowinCleanCode.Extension;
using BlowinCleanCode.Extension.SyntaxExtension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.CodeSmell
{
    public sealed class HollowTypeNameFeatureAnalyze : TypeDeclarationSyntaxNodeAnalyzerBase
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.HollowTypeName, 
            title: "Hollow type name",
            messageFormat: "'{0}' has a name that doesn't express its intent.", 
            Constant.Category.CodeSmell, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true);

        protected override void Analyze(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax syntaxNode)
        {
            var name = syntaxNode.TypeName();
            foreach (var (word, validateWhenFullMatch) in Settings.HollowTypeNameDictionary)
            {
                if(!validateWhenFullMatch && name.Equals(word))
                    continue;

                if (!name.EndsWith(word)) 
                    continue;
                
                ReportDiagnostic(context, syntaxNode.Identifier.GetLocation(), name);
                return;
            }
        }
    }
}