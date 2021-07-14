using System;

namespace BlowinCleanCode
{
    public class AnalyzerChainCallSettings
    {
        public bool MaxCallMustIncludeFluentInterfaceCall { get; set; }
        public int MaxCall { get; set; } = 5;
        public int? MaxFluentInterfaceCall { get; set; }
    }

    public class AnalyzerLargeClassSettings
    {
        public int MaxMethodThreshold { get; set; } = 10;
        public double NonPrivateMethodThreshold { get; set; } = 1;
        public double PrivateMethodThreshold { get; set; } = 0.55;

        public bool IsValid(int privateMethods, int nonPrivateMethods)
        {
            var threshold = CalculateThreshold(privateMethods, nonPrivateMethods);
            return threshold <= MaxMethodThreshold;
        }
        
        private double CalculateThreshold(int privateMethods, int nonPrivateMethods)
        {
            var privateThreshold = PrivateMethodThreshold * privateMethods;
            var nonPrivateThreshold = NonPrivateMethodThreshold * nonPrivateMethods;
            var amount = privateThreshold + nonPrivateThreshold;
            return (int)Math.Round(amount, MidpointRounding.AwayFromZero);
        }
    }
    
    public class AnalyzerSettings
    {
        public int MaxMethodDeclaration { get; set; } = 10;
        public int MaxCountOfLinesInMethod { get; set; } = 15;
        public int MaxMethodParameter { get; set; } = 4;
        public int MaxCountOfCondition { get; set; } = 4;
        public AnalyzerChainCallSettings ChainCallSettings { get; set; } = new AnalyzerChainCallSettings();
        public int MaxPreserveWholeObjectCount { get; set; } = 1;
        public AnalyzerLargeClassSettings LargeClass { get; set; } = new AnalyzerLargeClassSettings();
    }
}