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
        private readonly IContextLogger _logger;
        //private readonly IContextTracer _tracer;

        public HandleServiceBusMessage(
            IContextLogger logger,
            ITelemetryPipeline telemetry,
            IHeaderCarrierStrategy headerStrategy)
            : base(logger, telemetry, headerStrategy)
        { }

        [FunctionName("HandleServiceBusMessage")]
        public async Task Run(
            [ServiceBusTrigger(SbConstants.QueueNames.ProcessedItemsBinding, Connection = SbConstants.ConnectionStrings.Primary)]
            ServiceBusReceivedMessage message)
        {
            // Extract out-of-band headers (telemetry context) from SB application properties
            ITelemetryContext? restoredContext = GetContextHeader(message.ApplicationProperties as IDictionary<string, object>);

            if (restoredContext is not null)
            {
                ContextHolder<ITelemetryContext>.Current = restoredContext;
            }

            using var span = TraceScope("HandleServiceBusMessage");

            _logger.LogInformation("Running HandleServiceBusMessage");
            TraceMetric("contoso.sb.HandleServiceBusMessage.received", 1);

            try
            {
                var payload = JsonSerializer.Deserialize<MySbPayload>(message.Body);

                _logger.LogInformation(string.Format("Handled SB message: {0} (ID: {1})",
                    payload?.SomeAction,
                    payload?.SomeMessageId));

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                TraceError(ex);
                throw;
            }
        }
    }
}