using System;
using System.Diagnostics;
using Contoso.Infrastructure.Messaging;
using Contoso.Utilities.Logging;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Contoso.FunctionsApp.Startup))]

namespace Contoso.FunctionsApp;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

        // build...

        builder.Services
            .AddOptions<LoggingOptions>()
            .Bind(configuration.GetSection("Logging"))
            .ValidateDataAnnotations();

        // Register OpenTelemetry for Datadog
        builder.Services
            .AddLoggingWithTracing(configuration);


        // // Setup OpenTelemetry Activity
        // SetupOpenTelemetry();

        

        builder.Services
            .AddOptions<SbOptions>()
            // .Bind(configuration.GetSection("ServiceBus"));
            .Configure(options =>
            {
                options.ConnectionString = configuration[SbConstants.ConnectionStrings.Primary];
                options.QueueName = configuration[SbConstants.QueueNames.ProcessedItemsSetting];
            });


        // Add...
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton<IContextLogger, ContextLogger>();
        builder.Services.AddSingleton<IContextTracer, ContextTracer>();
        builder.Services.AddSingleton<IServiceBusEnvelopeSender, ServiceBusEnvelopeSender>();



    }

    private void SetupOpenTelemetry()
    {
        // Use W3C trace format for Azure compatibility
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        Activity.ForceDefaultIdFormat = true;

        ActivitySource activitySource = new("Contoso.FunctionsApp");

        ActivityListener listener = new()
        {
            ShouldListenTo = source => source.Name == "Contoso.FunctionsApp",
            Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStarted = activity => { },
            ActivityStopped = activity =>
            {
                // Optional: flush to console or Application Insights
                Console.WriteLine($"[Trace] {activity.DisplayName} | TraceId: {activity.TraceId} | SpanId: {activity.SpanId}");
            }
        };

        ActivitySource.AddActivityListener(listener);
    }
}