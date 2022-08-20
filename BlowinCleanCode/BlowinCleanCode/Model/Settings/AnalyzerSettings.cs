namespace BlowinCleanCode.Model.Settings
{
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
    }
}