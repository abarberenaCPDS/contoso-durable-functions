using System;
using System.Threading.Tasks;
using Contoso.Infrastructure.Context;
using Contoso.Utilities.Logging;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using System.Text.Json;
using Contoso.Infrastructure.Messaging;

namespace Contoso.FunctionsApp
{
    public class HandleServiceBusMessage
    {
        private readonly IContextLogger _logger;

        public HandleServiceBusMessage(IContextLogger logger)
        {
            _logger = logger;
        }

        [FunctionName("HandleServiceBusMessage")]

        public async Task Run(
            // [ServiceBusTrigger("%ServiceBusQueueName%", Connection = "ServiceBus:ConnectionString")]
            [ServiceBusTrigger(SbConstants.QueueNames.ProcessedItemsBinding, Connection = SbConstants.ConnectionStrings.Primary)]
         ServiceBusReceivedMessage message)
        {
            // Extract out-of-band headers
            var ctx = new MyAppContext
            {
                ApplicationId = message.ApplicationProperties.TryGetValue("x-app-id", out var appId) ? appId?.ToString() : null,
                UserCode = message.ApplicationProperties.TryGetValue("x-user-code", out var userCode) ? userCode?.ToString() : null,
                OrchestrationId = message.ApplicationProperties.TryGetValue("x-orch-id", out var orchId) ? orchId?.ToString() : null
            };

            var tx = new DistributedTransactionContext
            {
                TransactionId = message.ApplicationProperties.TryGetValue("x-tx-id", out var txId) ? txId?.ToString() : null,
                CurrentStep = message.ApplicationProperties.TryGetValue("x-tx-step", out var txStep) ? txStep?.ToString() : null,
                Status = "Received"
            };

            ContextHolder<MyAppContext>.Current = ctx;
            ContextHolder<DistributedTransactionContext>.Current = tx;

            using var span = ActivityTracing.StartSpan("HandleServiceBusMessage");
            TransactionEnricher.MarkStep("HandleServiceBusMessage");

            // Deserialize message body
            var payload = JsonSerializer.Deserialize<MySbPayload>(message.Body);

            _logger.LogInfo($"Handled Service Bus message: {payload?.SomeAction} (ID: {payload?.SomeMessageId})");

            await Task.CompletedTask;

        }
    }
}