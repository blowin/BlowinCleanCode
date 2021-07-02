namespace BlowinCleanCode
{
    public class AnalyzerChainCallSettings
    {
        public bool MaxCallMustIncludeFluentInterfaceCall { get; set; }
        public int MaxCall { get; set; } = 5;
        public int? MaxFluentInterfaceCall { get; set; }
    }
    
    public class AnalyzerSettings
    {
        public int MaxMethodDeclaration { get; set; } = 10;
        public int MaxCountOfLinesInMethod { get; set; } = 15;
        public int MaxMethodParameter { get; set; } = 4;
        public int MaxCountOfCondition { get; set; } = 4;
        public AnalyzerChainCallSettings ChainCallSettings { get; set; } = new AnalyzerChainCallSettings();
    }
}