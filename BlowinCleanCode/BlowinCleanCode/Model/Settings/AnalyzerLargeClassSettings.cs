using System;

namespace BlowinCleanCode.Model.Settings
{
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
}