using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using BlowinCleanCode.Feature;
using BlowinCleanCode.Feature.CodeSmell;
using BlowinCleanCode.Feature.CodeSmell.MagicValue;
using BlowinCleanCode.Feature.GoodPractice;
using BlowinCleanCode.Feature.GoodPractice.Disposable;
using BlowinCleanCode.Feature.SingleResponsibility;

namespace BlowinCleanCode
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BlowinCleanCodeAnalyzer : DiagnosticAnalyzer
    {
        private static readonly IFeature[] Features = 
        {
            // Encapsulation
            new PublicStaticFieldFeatureSymbolAnalyze(),

            // Single responsibility
            new LongMethodFeatureSymbolAnalyze(),
            new ManyParameterFeatureSymbolAnalyze(),
            new ControlFlagFeatureAnalyze(),
            new MethodContainSymbolAnalyzeAndFeatureSymbolAnalyze(),
            new MethodALotOfDeclarationFeatureAnalyze(),
            new LargeTypeFeatureAnalyze(),
            new LargeNumberOfFieldsFeatureAnalyze(),
            new LambdaHaveTooManyLinesFeatureAnalyze(),
            
            // Good practice
            new ReturnNullFeatureSymbolAnalyze(),
            new StaticClassFeatureSymbolAnalyze(),
            new DisposableMemberInNonDisposableClassFeatureAnalyze(),
            new SwitchStatementsShouldHaveAtLeast2CaseClausesFeatureAnalyze(),
            new FinalizersShouldNotBeEmptyFeatureAnalyze(),
            new TypeThatProvideEqualsShouldImplementIEquatableFeatureAnalyze(),
            
            // Code smells
            new NestedTernaryOperatorFeatureAnalyze(),
            new MagicValueFeatureSymbolAnalyze(),
            new ComplexConditionFeatureAnalyze(),
            new PreserveWholeObjectFeatureAnalyze(),
            new HollowTypeNameFeatureAnalyze(),
            new DeeplyNestedCodeFeatureAnalyze(),
            new LongChainCallFeatureAnalyze(),
            new MethodShouldNotHaveManyReturnStatementsFeatureAnalyze(),
            new SwitchShouldNotHaveALotOfCasesFeatureAnalyze(),
            new SwitchStatementsShouldNotBeNestedFeatureAnalyze(),
            new CatchShouldDoMoreThanRethrowFeatureAnalyze(),
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
