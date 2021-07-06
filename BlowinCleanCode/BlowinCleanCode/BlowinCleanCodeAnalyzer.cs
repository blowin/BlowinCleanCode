using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using BlowinCleanCode.Feature;
using BlowinCleanCode.Feature.MethodContain;

namespace BlowinCleanCode
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BlowinCleanCodeAnalyzer : DiagnosticAnalyzer
    {
        private static readonly IFeature[] Features = 
        {
            new PublicStaticFieldFeatureSymbolAnalyze(),

            new LongMethodFeatureSymbolAnalyze(),
            new ManyParameterFeatureSymbolAnalyze(),
            new ControlFlagFeatureAnalyze(),
            new MethodContainSymbolAnalyzeAndFeatureSymbolAnalyze(),
            new MethodALotOfDeclarationFeatureAnalyze(),
            new MagicValueFeatureSymbolAnalyze(),
            new ComplexConditionFeatureAnalyze(),
            
            new ValueReturnNullFeatureSymbolAnalyze(),
            new StaticClassFeatureSymbolAnalyze(),
            new LongChainCallFeatureAnalyze(),
            
            new NestedTernaryOperatorFeatureAnalyze(),
        };

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                var builder = ImmutableArray.CreateBuilder<DiagnosticDescriptor>(Features.Length);
                
                foreach (var feature in Features)
                    builder.Add(feature.DiagnosticDescriptor);

                return builder.ToImmutable();
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            
            foreach (var feature in Features)
                feature.Register(context);
        }
    }
}
