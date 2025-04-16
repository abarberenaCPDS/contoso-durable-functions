using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Contoso.Infrastructure.Context;
using Contoso.Infrastructure.Core;
using Contoso.Infrastructure.Messaging;
using Contoso.Utilities.Context;
using Contoso.Utilities.Logging;
using ContosoFunctionsApp.Extensions;
using Microsoft.Azure.WebJobs;

namespace Contoso.FunctionsApp
{
    public class HandleServiceBusMessage : FunctionExtension
    {
        public HandleServiceBusMessage(
            IContextLogger logger,
            ITelemetryPipeline telemetryPipeline,
            IHeaderCarrierStrategy headerStrategy)
            : base(logger, telemetryPipeline, headerStrategy)
        { }

        [FunctionName("HandleServiceBusMessage")]
        public async Task Run(
            [ServiceBusTrigger(SbConstants.QueueNames.ProcessedItemsBinding, Connection = SbConstants.ConnectionStrings.Primary)]
            ServiceBusReceivedMessage message)
        {
            // Extract out-of-band headers (telemetry context) from SB application properties

            // ContextHolder must be set before calling FunctionWrapper.HandleAsync(...)
            // this ensures that telemetry, logging, and exceptions are enriched
            ITelemetryContext? telemetryContext = TelemetryContextBinder.CreateFromHeaders(message.ApplicationProperties as IDictionary<string, object>);

            // TODO: for demo only
            if (telemetryContext is null)
            {
                ArgumentNullException telemetryContextNullException = new ArgumentNullException("TelemetryContext is empty.");
                TraceError(telemetryContextNullException);
                this._logger.LogError(telemetryContextNullException);
                throw telemetryContextNullException;
            }

            await RunWithHandling(async () =>
            {
                (telemetryContext as OrchestrationInputContext).DistributedTransactionContext.CurrentStep = "HandleServiceBusMessage";
                ContextHolder<ITelemetryContext>.Current = telemetryContext;
                _logger.LogInformation("Running HandleServiceBusMessage");

                using var span = TraceScope("HandleServiceBusMessage");

                _logger.LogInformation("Running HandleServiceBusMessage");
                TraceMetric("contoso.sb.HandleServiceBusMessage.received", 1);

                var payload = JsonSerializer.Deserialize<MySbPayload>(message.Body);

                _logger.LogInformation($"Handled SB message --> SomeAction: {payload?.SomeAction} - SomeMessageId: {payload?.SomeMessageId})");

                await Task.CompletedTask;
            }, "HandleServiceBusMessage.Run");
        }
    }
}