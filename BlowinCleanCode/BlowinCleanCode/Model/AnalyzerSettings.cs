using System;

namespace BlowinCleanCode.Model
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
        public int MaxLengthVariableName { get; set; } = 26;

        public int MaxNumberOfField { get; set; } = 5;

        public int MaxDeeplyNested { get; set; } = 3;

        public static AnalyzerSettings Instance { get; } = new AnalyzerSettings();
        
        public int MaxMethodDeclaration { get; set; } = 10;

        public int MaxCountOfLinesInMethod { get; set; } = 21;

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