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
        private readonly IServiceBusEnvelopeSender _serviceBusEnvelopeSender;
        private readonly SbOptions _sbOptions;

        public ProcessActivity(IContextLogger logger, IServiceBusEnvelopeSender serviceBusEnvelopeSender, IOptions<SbOptions> sbOptions)
        {
            _logger = logger;
            this._serviceBusEnvelopeSender = serviceBusEnvelopeSender;
            this._sbOptions = sbOptions.Value;
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

            // // approach 1: Sending a Message
            // await SendMessageAsync(messageBody);

            // approach 2: Sending an Envelope
            await _serviceBusEnvelopeSender.SendAsync(messageBody, input.MyAppContext, input.DistributedTransactionContext);
            _logger.LogInfo($"Message sent to Service Bus queue: {_sbOptions.QueueName}");
            await Task.CompletedTask;
        }

        private async Task SendMessageAsync(MySbPayload messageBody)
        {
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
        }
    }
}