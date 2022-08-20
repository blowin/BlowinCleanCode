namespace BlowinCleanCode.Model.Settings
{
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
}