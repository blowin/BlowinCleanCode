namespace BlowinCleanCode
{
    public class AnalyzerSettings
    {
        public int MaxMethodDeclaration { get; set; } = 10;
        public int MaxCountOfLinesInMethod { get; set; } = 15;
        public int MaxMethodParameter { get; set; } = 4;
        public int MaxCountOfCondition { get; set; } = 4;
    }
}