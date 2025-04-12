using System.Configuration;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Contoso.Utilities.Logging
{
    public static class LoggingStartupExtensions
    {
        public static IServiceCollection AddLoggingWithTracing(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<LoggingOptions>(config.GetSection("Logging"));
            var telemetryOptions = config.GetSection("Logging").Get<LoggingOptions>();

            services.AddLogging(loggingBuilder =>
            {
                if (telemetryOptions.EnableConsoleLogging)
                    loggingBuilder.AddConsole();

                loggingBuilder.AddOpenTelemetry(options =>
                {
                    options.IncludeScopes = true;
                    options.IncludeFormattedMessage = true;
                    options.ParseStateValues = true;

                    if (telemetryOptions.EnableConsoleLogging)
                        options.AddConsoleExporter();

                    if (telemetryOptions.EnableAppInsights)
                    {
                        options.AddAzureMonitorLogExporter(o =>
                        {
                            o.ConnectionString = telemetryOptions.AppInsightsConnectionString;
                        });
                    }
                });
            });


            string exporter = config["Logging:TelemetryExporter"];

            services.AddOpenTelemetry()
                .ConfigureResource(resource =>
                {
                    resource
                        .AddService(TracingConstants.ServiceName)
                        .AddTelemetrySdk();
                })
                .WithTracing(tracing =>
                {
                    //tracing
                    //    .AddSource(TracingConstants.SourceName)
                    //    .SetResourceBuilder(
                    //        ResourceBuilder.CreateDefault()
                    //            .AddService(TracingConstants.ServiceName)
                    //            .AddAttributes(new[]
                    //            {
                    //                new KeyValuePair<string, object>("deployment.environment", TracingConstants.Environment),
                    //                new KeyValuePair<string, object>("team", TracingConstants.Team)
                    //            }))
                    //    .AddAspNetCoreInstrumentation()
                    //    .AddHttpClientInstrumentation();

                    //if (exporter == "DataDog")
                    //{
                    //    tracing.AddOtlpExporter(o =>
                    //    {
                    //        o.Endpoint = new Uri(config["Logging:DataDog:Endpoint"]);
                    //        o.Protocol = OtlpExportProtocol.HttpProtobuf;
                    //        o.Headers = $"DD-API-KEY={config["Logging:DataDog:ApiKey"]}";
                    //    });
                    //}

                    tracing
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddSource(TracingConstants.ServiceName);

                    if (telemetryOptions.EnableConsoleLogging)
                        tracing.AddConsoleExporter();

                    if (telemetryOptions.EnableDatadog && !string.IsNullOrEmpty(telemetryOptions.DatadogExporterEndpoint))
                    {
                        tracing.AddOtlpExporter(o =>
                        {
                            o.Endpoint = new Uri(telemetryOptions.DatadogExporterEndpoint);
                        });
                    }

                    if (telemetryOptions.EnableAppInsights)
                    {
                        tracing.AddAzureMonitorTraceExporter(o =>
                        {
                            o.ConnectionString = telemetryOptions.AppInsightsConnectionString;
                        });
                    }

                    if (config.GetValue<bool>("Logging:EnableConsoleExporter"))
                    {
                        tracing.AddConsoleExporter();
                    }
                })
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddAspNetCoreInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddMeter(TracingConstants.ServiceName);

                    if (telemetryOptions.EnableConsoleLogging)
                        metrics.AddConsoleExporter();

                    if (telemetryOptions.EnableDatadog && !string.IsNullOrEmpty(telemetryOptions.DatadogExporterEndpoint))
                    {
                        metrics.AddOtlpExporter(o =>
                        {
                            o.Endpoint = new Uri(telemetryOptions.DatadogExporterEndpoint);
                        });
                    }

                    if (telemetryOptions.EnableAppInsights)
                    {
                        metrics.AddAzureMonitorMetricExporter(o =>
                        {
                            o.ConnectionString = telemetryOptions.AppInsightsConnectionString;
                        });
                    }
                }); ;

            // Register IContextTracer so that spans can be created with context tagging
            services.AddSingleton<IContextTracer, ContextTracer>();

            return services;
        }
    }
}