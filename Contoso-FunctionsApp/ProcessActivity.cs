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
using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;

namespace Contoso.FunctionsApp
{
    public class ProcessActivity
    {
        private readonly IContextLogger _logger;
        private readonly IServiceBusEnvelopeSender _serviceBusEnvelopeSender;
        private readonly SbOptions _sbOptions;


        private static readonly ActivitySource ActivitySource = new(TracingConstants.ServiceName);
        private static readonly Meter Meter = new(TracingConstants.ServiceName);
        private static readonly Counter<long> RequestCounter = Meter.CreateCounter<long>("contoso.process.requests");



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

            using var activity = ActivitySource.StartActivity("ProcessActivity.Run");

            activity?.SetTag("app.applicationid", ContextHolder<MyAppContext>.Current.ApplicationId);
            activity?.SetTag("app.usercode", ContextHolder<MyAppContext>.Current.UserCode);
            activity?.SetTag("app.orchestrationid", ContextHolder<MyAppContext>.Current.OrchestrationId);
            activity?.SetTag("tx.id", ContextHolder<DistributedTransactionContext>.Current.TransactionId);
            activity?.SetTag("tx.current.step", ContextHolder<DistributedTransactionContext>.Current.CurrentStep);
            activity?.SetTag("tx.status", ContextHolder<DistributedTransactionContext>.Current.Status);

            _logger.LogInfo("Processing ProcessActivity.Run");

            RequestCounter.Add(1, new KeyValuePair<string, object?>("environment", "local"));
            RequestCounter.Add(1, new KeyValuePair<string, object?>("Step", "ProcessActivity"));

            try
            {
                // // approach 1: Sending a Message
                // await SendMessageAsync(messageBody);

                if (DateTime.Now.Millisecond % 2 == 1)
                    throw new InvalidOperationException("InvalidOperationException, this an intended exception ");

                // approach 2: Sending an Envelope
                await _serviceBusEnvelopeSender.SendAsync(messageBody, input.MyAppContext, input.DistributedTransactionContext);
                _logger.LogInfo($"Message sent to Service Bus queue: {_sbOptions.QueueName}");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                _logger.LogError(ex, "Error occurred in ProcessActivity");
                throw;
            }
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