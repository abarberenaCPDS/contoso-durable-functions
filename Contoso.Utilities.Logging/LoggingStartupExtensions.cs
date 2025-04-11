using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Contoso.Utilities.Logging
{
    public static class LoggingStartupExtensions
    {
        public static IServiceCollection AddLoggingWithTracing(this IServiceCollection services, IConfiguration config)
        {
            string exporter = config["Logging:TelemetryExporter"];

            services.AddOpenTelemetry()
                .WithTracing(builder =>
                {
                    builder
                        .AddSource(TracingConstants.SourceName)
                        .SetResourceBuilder(
                            ResourceBuilder.CreateDefault()
                                .AddService(TracingConstants.ServiceName)
                                .AddAttributes(new[]
                                {
                                    new KeyValuePair<string, object>("deployment.environment", TracingConstants.Environment),
                                    new KeyValuePair<string, object>("team", TracingConstants.Team)
                                }))
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation();

                    if (exporter == "DataDog")
                    {
                        builder.AddOtlpExporter(o =>
                        {
                            o.Endpoint = new Uri(config["Logging:DataDog:Endpoint"]);
                            o.Protocol = OtlpExportProtocol.HttpProtobuf;
                            o.Headers = $"DD-API-KEY={config["Logging:DataDog:ApiKey"]}";
                        });
                    }

                    if (config.GetValue<bool>("Logging:EnableConsoleExporter"))
                    {
                        builder.AddConsoleExporter();
                    }
                });

            // Register IContextTracer so that spans can be created with context tagging
            services.AddSingleton<IContextTracer, ContextTracer>();

            return services;
        }
    }
}