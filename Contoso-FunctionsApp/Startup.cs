using System;
using Contoso.Infrastructure.Core;
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
        // builder.Services.AddSingleton<IContextTracer, ContextTracer>();
        builder.Services.AddSingleton<IHeaderCarrierStrategy, HeaderCarrierStrategy>();
        builder.Services.AddSingleton<ITelemetryPipeline, TelemetryPipeline>();
        builder.Services.AddSingleton<ITelemetryEnricher, TelemetryEnricher>();
        builder.Services.AddSingleton<IServiceBusEnvelopeSender, ServiceBusEnvelopeSender>();

    }
}