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

    public class CognitiveComplexitySettings
    {
        public int MinLowComplexity { get; set; } = 8;
        public int MinMiddleComplexity { get; set; } = 10;
        public int MinHighComplexity { get; set; } = 15;

        public bool TryBuildMessage(int actualComplexity, out string message)
        {
            message = string.Empty;
            if (actualComplexity >= MinLowComplexity && actualComplexity < MinMiddleComplexity)
            {
                message = $"Low complexity. Actual complexity {actualComplexity}";
                return true;
            }

            if (actualComplexity >= MinMiddleComplexity && actualComplexity < MinHighComplexity)
            {
                message = $"Middle complexity. Actual complexity {actualComplexity}";
                return true;
            }

            if (actualComplexity >= MinHighComplexity)
            {
                message = $"High complexity, should be refactored. Actual complexity {actualComplexity}";
                return true;
            }

            return false;
        }
    }

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