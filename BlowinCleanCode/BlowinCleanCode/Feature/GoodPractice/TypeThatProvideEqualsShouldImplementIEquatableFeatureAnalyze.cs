using System.Collections.Generic;
using System.Linq;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.GoodPractice
{
    public sealed class TypeThatProvideEqualsShouldImplementIEquatableFeatureAnalyze : TypeDeclarationSyntaxNodeAnalyzerBase
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.TypeThatProvideEqualsShouldImplementIEquatable, 
            title: "Type that provide Equals should implement IEquatable",
            messageFormat: "Classes that provide \"Equals({0})\" should implement \"IEquatable<{0}>\"", 
            Constant.Category.GoodPractice, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true);
        
        protected override void Analyze(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax syntaxNode)
        {
            var location = syntaxNode.Identifier.GetLocation();
            
            var equalTypes = GetAllEqualTypes(context, syntaxNode);
            var implementTypes = GetImplementEqualsTypes(context, syntaxNode);
            foreach (var typeSyntax in equalTypes.Except(implementTypes))
                ReportDiagnostic(context, location, typeSyntax.Name);
        }

        private static IEnumerable<ITypeSymbol> GetImplementEqualsTypes(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax syntaxNode)
        {
            if(syntaxNode.BaseList == null)
                yield break;

            foreach (var baseTypeSyntax in syntaxNode.BaseList.Types)
            {
                if(!baseTypeSyntax.Type.Is<GenericNameSyntax>(out var type))
                    continue;

                var genericParameter = GetEquatableGenericArgument(context, type);
                if(genericParameter == null)
                    continue;

                yield return context.SemanticModel.GetTypeInfo(genericParameter).Type;
            }
        }
        
        private IEnumerable<ITypeSymbol> GetAllEqualTypes(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax syntax)
        {
            foreach (var node in syntax.Members)
            {
                if (SkipCheckMemberDeclaration(node, out var paramType)) 
                    continue;

                yield return context.SemanticModel.GetTypeInfo(paramType).Type;
            }
        }

        private bool SkipCheckMemberDeclaration(MemberDeclarationSyntax node, out TypeSyntax paramType)
        {
            paramType = default;
            
            if (!node.Is<MethodDeclarationSyntax>(out var methodSyntax))
                return true;

            if (AnalyzerCommentSkipCheck.Skip(methodSyntax) || methodSyntax.ParameterList == null)
                return true;

            var parameters = methodSyntax.ParameterList.Parameters;
            if (methodSyntax.Identifier.ToFullString() != "Equals" || parameters.Count != 1)
                return true;

            paramType = parameters.First().Type;

            // Equals(object)
            if (paramType is PredefinedTypeSyntax pts && pts.Keyword.IsKind(SyntaxKind.ObjectKeyword))
                return true;
            
            return false;
        }

        private static TypeSyntax GetEquatableGenericArgument(SyntaxNodeAnalysisContext context, GenericNameSyntax type)
        {
            if (type.Identifier.ToString() != "IEquatable")
                return null;

            return type.TypeArgumentList?.Arguments.FirstOrDefault();
        }
    }
}