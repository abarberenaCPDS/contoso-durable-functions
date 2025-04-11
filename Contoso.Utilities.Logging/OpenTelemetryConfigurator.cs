using System.Reflection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Contoso.Utilities.Logging
{
    public static class OpenTelemetryConfigurator
    {
        public static TracerProvider Configure()
        {
            var resourceBuilder = ResourceBuilder.CreateDefault()
                .AddService(serviceName: TracingConstants.ServiceName,
                            serviceVersion: Assembly.GetExecutingAssembly().GetName().Version?.ToString())
                .AddAttributes(new[]
                {
                    new KeyValuePair<string, object>("deployment.environment", TracingConstants.Environment),
                    new KeyValuePair<string, object>("team", TracingConstants.Team)
                });

            return Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(resourceBuilder)
                .AddSource(TracingConstants.SourceName)
                .AddOtlpExporter(opt =>
                {
                    opt.Endpoint = new Uri("https://otel.datadoghq.com");
                    opt.Headers = $"DD-API-KEY={Environment.GetEnvironmentVariable("DD_API_KEY")}";
                })
                .Build();
        }
    }
}
