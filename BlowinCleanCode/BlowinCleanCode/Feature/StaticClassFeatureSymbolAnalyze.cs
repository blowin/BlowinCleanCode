using System.Linq;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature
{
    public sealed class StaticClassFeatureSymbolAnalyze : FeatureSyntaxNodeAnalyzerBase<ClassDeclarationSyntax>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.StaticClass, 
            title: "Class can't be static",
            messageFormat: "Class '{0}' must be non static", 
            Constant.Category.GoodPractice, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true,
            description: "Static class is bad practice. if you can't do without a static class, use singleton pattern.");

        protected override SyntaxKind SyntaxKind => SyntaxKind.ClassDeclaration;
        
        protected override void Analyze(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax syntaxNode)
        {
            if(!syntaxNode.Modifiers.Any(SyntaxKind.StaticKeyword))
                return;

            var identifier = syntaxNode.Identifier;
            if(string.IsNullOrEmpty(identifier.Text))
                return;

            foreach (var memberDeclarationSyntax in syntaxNode.Members)
            {
                if(!(memberDeclarationSyntax is MethodDeclarationSyntax mds))
                    continue;
                
                if(IsExtension(mds))
                    continue;
                
                ReportDiagnostic(context, identifier.GetLocation(), identifier.Text);
                return;
            }
        }

        private static bool IsExtension(MethodDeclarationSyntax methodDeclarationSyntax)
        {
            var parameters = methodDeclarationSyntax.ParameterList.Parameters;
            if (parameters.Count == 0)
                return false;

            return parameters[0].Modifiers.Any(SyntaxKind.ThisKeyword);
        }
    }
}