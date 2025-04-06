using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Contoso.Infrastructure.Context;
using Contoso.Utilities.Logging;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Options;
using Contoso.Infrastructure.Messaging;
using System.Text.Json;
using System.Text;

namespace Contoso.FunctionsApp
{
    public class ProcessActivity
    {
        private readonly IContextLogger _logger;
        private readonly SbOptions _sbOptions;

        public ProcessActivity(IContextLogger logger, IOptions<SbOptions> sbOptions)
        {
            _logger = logger;
            _sbOptions = sbOptions.Value;
        }

        [FunctionName("ProcessActivity")]
        public async Task Run([ActivityTrigger] OrchestrationInput input)
        {
            // retrieve context...
            ContextHolder<MyAppContext>.Current = input.MyAppContext;
            ContextHolder<DistributedTransactionContext>.Current = input.DistributedTransactionContext;

            // log in OpenTelemetry...
            using var span = ActivityTracing.StartSpan("ProcessActivity");
            TransactionEnricher.MarkStep("ProcessActivity");

            _logger.LogInfo("Running ProcessActivity — sending message to Service Bus");

            var messageBody = new MySbPayload
            {
                SomeMessageId = Guid.NewGuid().ToString(),
                SomeAction = "ProcessedData",
                ATimestamp = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(messageBody);
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(json))
            {
                MessageId = messageBody.SomeMessageId
            };
            
            // Add context as headers (out-of-band)
            var ctx = ContextHolder<MyAppContext>.Current;
            var tx = ContextHolder<DistributedTransactionContext>.Current;

            message.ApplicationProperties["x-app-id"] = ctx?.ApplicationId;
            message.ApplicationProperties["x-user-code"] = ctx?.UserCode;
            message.ApplicationProperties["x-orch-id"] = ctx?.OrchestrationId;
            message.ApplicationProperties["x-tx-id"] = tx?.TransactionId;
            message.ApplicationProperties["x-tx-step"] = tx?.CurrentStep;

            await using var client = new ServiceBusClient(_sbOptions.ConnectionString);
            var sender = client.CreateSender(_sbOptions.QueueName);
            
            // Send the message
            await sender.SendMessageAsync(message);

            _logger.LogInfo($"Message sent to Service Bus queue: {_sbOptions.QueueName}");
            // await Task.CompletedTask;
        }
    }
}