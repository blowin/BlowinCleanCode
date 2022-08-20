namespace BlowinCleanCode.Model.Settings
{
    public class AnalyzerChainCallSettings
    {
        public bool MaxCallMustIncludeFluentInterfaceCall { get; set; }
        public int MaxCall { get; set; } = 5;
        public int? MaxFluentInterfaceCall { get; set; }
    }
}