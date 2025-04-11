namespace Contoso.Utilities.Logging
{
    public static class TracingConstants
    {
        public const string SourceName = "ContosoTracer"; // must match ActivitySource
        public const string ServiceName = "Contoso.FunctionsApp";
        public const string Environment = "local"; //"dev", "qa", etc...override via env var if needed
        public const string Team = "Asgardians";
    }
}