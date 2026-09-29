using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Model.Settings
{
    /// <summary>
    /// Compiled-in defaults of every tunable analyzer option.
    /// <para>
    /// An analyzer does not read these properties directly. It calls
    /// <see cref="Resolve(Microsoft.CodeAnalysis.Diagnostics.SyntaxNodeAnalysisContext)"/> (or one of its
    /// overloads), which returns an <see cref="AnalyzerSettingsReader"/> merging these defaults with the
    /// values the user set in '.editorconfig'. That reader is the single entry point for options; the
    /// list of keys lives in <see cref="Constant.Option"/>.
    /// </para>
    /// </summary>
    public class AnalyzerSettings
    {
        public int MaxNameLength { get; set; } = 26;

        public int MaxNumberOfField { get; set; } = 5;

        public int MaxDeeplyNested { get; set; } = 3;

        public static AnalyzerSettings Instance { get; } = new AnalyzerSettings();
        
        public int MaxMethodDeclaration { get; set; } = 10;

        public CognitiveComplexitySettings CognitiveComplexity { get; set; } = new CognitiveComplexitySettings();

        public int MaxCountOfLinesInMethod { get; set; } = 25;

        public int MaxLambdaCountOfLines { get; set; } = 10;

        public int MaxMethodParameter { get; set; } = 4;

        public int MaxCountOfCondition { get; set; } = 4;

        public AnalyzerChainCallSettings ChainCallSettings { get; set; } = new AnalyzerChainCallSettings();

        public int MaxPreserveWholeObjectCount { get; set; } = 2;

        public AnalyzerLargeClassSettings LargeClass { get; set; } = new AnalyzerLargeClassSettings();

        public int MaxReturnStatement { get; set; } = 4;

        public int MaxReturnStatementForReturnBool { get; set; } = 8;

        public int MaxSwitchCaseCount { get; set; } = 4;

        public (string Word, bool ValidateWhenFullMatch)[] HollowTypeNameDictionary { get; set; } = {
            ("Helper", true),
            ("Util", true),
            ("Utils", true),
            ("Utility", true),
            ("Utilities", true),
            ("Info", true),
            ("Data", true),
            ("Manager", false),
        };

        /// <summary>
        /// Single entry point for reading an option. Returns the effective values for the file that
        /// contains the analyzed node: the compiled-in defaults overridden by what the user set in
        /// '.editorconfig' for that file.
        /// </summary>
        public AnalyzerSettingsReader Resolve(SyntaxNodeAnalysisContext context)
            => Resolve(context.Options, context.Node?.SyntaxTree);

        /// <summary>
        /// Single entry point for reading an option from a symbol-based analyzer. Options are resolved
        /// from the file that declares the symbol, so a per-directory '.editorconfig' is honoured.
        /// </summary>
        public AnalyzerSettingsReader Resolve(SymbolAnalysisContext context)
            => Resolve(context.Options, DeclaringSyntaxTree(context.Symbol));

        /// <summary>
        /// Single entry point for reading an option when only the options and the file are known.
        /// When there is no configuration for the file, the compiled-in defaults are returned.
        /// </summary>
        public AnalyzerSettingsReader Resolve(AnalyzerOptions options, SyntaxTree syntaxTree)
        {
            var provider = options?.AnalyzerConfigOptionsProvider;
            if (provider == null || syntaxTree == null)
                return new AnalyzerSettingsReader(this, null);

            return new AnalyzerSettingsReader(this, provider.GetOptions(syntaxTree));
        }

        private static SyntaxTree DeclaringSyntaxTree(ISymbol symbol)
        {
            if (symbol == null)
                return null;

            foreach (var location in symbol.Locations)
            {
                if (location.SourceTree != null)
                    return location.SourceTree;
            }

            return null;
        }
    }
}