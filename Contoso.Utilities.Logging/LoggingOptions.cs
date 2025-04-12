namespace Contoso.Utilities.Logging
{
    public class LoggingOptions
    {
        public bool EnableConsoleLogging { get; set; }
        public bool EnableAppInsights { get; set; }
        public bool EnableDatadog { get; set; }
        public string? AppInsightsConnectionString { get; set; }
        public string? DatadogExporterEndpoint { get; set; }
        public bool UseConsoleColors { get; set; }
    }
}
