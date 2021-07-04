using System;
using BlowinCleanCode.Extension;
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

            int countOfMethod = 0;
            var hasMainMethod = false;
            foreach (var memberDeclarationSyntax in syntaxNode.Members)
            {
                if(!(memberDeclarationSyntax is MethodDeclarationSyntax mds))
                    continue;

                if (IsMainMethod(mds))
                {
                    hasMainMethod = true;
                    continue;
                }
                
                countOfMethod += 1;
                if(mds.IsExtension())
                    continue;
                
                ReportDiagnostic(context, identifier);
                return;
            }
            
            if(!hasMainMethod && countOfMethod == 0)
                ReportDiagnostic(context, identifier);
        }

        private void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxToken token)
            => ReportDiagnostic(context, token.GetLocation(), token.Text);

        private static bool IsMainMethod(MethodDeclarationSyntax methodDeclarationSyntax)
        {
            // void Main(string[] args)
            if (!methodDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword))
                return false;

            if (!(methodDeclarationSyntax.ReturnType is PredefinedTypeSyntax pts))
                return false;

            if (!pts.Keyword.IsKind(SyntaxKind.VoidKeyword))
                return false;

            var methodName = methodDeclarationSyntax.Identifier.ToFullString();
            if (!"Main".Equals(methodName))
                return false;
            
            var parameters = methodDeclarationSyntax.ParameterList.Parameters;
            if (parameters.Count != 1)
                return false;

            if(!(parameters[0].Type is ArrayTypeSyntax ats))
                return false;

            var type = ats.ElementType.ToFullString();
            return "string".Equals(type, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}