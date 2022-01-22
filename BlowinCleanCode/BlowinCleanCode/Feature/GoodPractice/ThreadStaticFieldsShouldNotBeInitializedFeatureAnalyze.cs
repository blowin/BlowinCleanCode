using System.Linq;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Feature.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.GoodPractice
{
    public sealed class ThreadStaticFieldsShouldNotBeInitializedFeatureAnalyze : FeatureSymbolAnalyzeBase<IFieldSymbol>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(Constant.Id.ThreadStaticFieldsShouldNotBeInitialized, 
            title: "\"ThreadStatic\" field should not be initialized",
            messageFormat: "{0} should not be initialized", 
            Constant.Category.GoodPractice, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true);

        protected override SymbolKind SymbolKind => SymbolKind.Field;
        
        protected override void Analyze(SymbolAnalysisContext context, IFieldSymbol symbol)
        {
            if (!HasAttribute(symbol) || !HasInitializer(symbol)) 
                return;
            
            var name = symbol.NormalizeName();
            ReportDiagnostic(context, symbol.Locations.First(), name);
        }

        private static bool HasAttribute(IFieldSymbol symbol)
        {
            foreach (var attributeData in symbol.GetAttributes())
            {
                if(attributeData.IsThreadStatic())
                    return true;
            }
            
            return false;
        }

        private static bool HasInitializer(IFieldSymbol symbol)
        {
            foreach (var syntaxRef in symbol.DeclaringSyntaxReferences)
            {
                var syntax = syntaxRef.GetSyntax();
                if (syntax is VariableDeclaratorSyntax declarator && declarator.Initializer != null)
                    return true;
            }

            return false;
        }
    }
}