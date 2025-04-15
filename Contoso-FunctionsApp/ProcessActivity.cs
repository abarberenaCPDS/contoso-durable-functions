using System;
using System.Threading.Tasks;
using Contoso.Infrastructure.Context;
using Contoso.Infrastructure.Core;
using Contoso.Infrastructure.Messaging;
using Contoso.Utilities.Context;
using Contoso.Utilities.Logging;
using ContosoFunctionsApp.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Options;

namespace Contoso.FunctionsApp
{
    public class ProcessActivity : FunctionExtension
    {
        private readonly IServiceBusEnvelopeSender _serviceBusEnvelopeSender;
        private readonly SbOptions _sbOptions;

        public ProcessActivity(
            IContextLogger logger,
            ITelemetryPipeline telemetryPipeline,
            IServiceBusEnvelopeSender serviceBusEnvelopeSender,
            IOptions<SbOptions> sbOptions,
            IHeaderCarrierStrategy headerStrategy)
            : base(logger, telemetryPipeline, headerStrategy)
        {
            _serviceBusEnvelopeSender = serviceBusEnvelopeSender;
            _sbOptions = sbOptions.Value;
        }

        [FunctionName("ProcessActivity")]
        public async Task Run([ActivityTrigger] OrchestrationInputContext input)
        {
            await RunWithHandling(async () =>
            {
                // update context...
                input.DistributedTransactionContext.CurrentStep = "ProcessActivity";
                ContextHolder<ITelemetryContext>.Current = input;

                using var span = TraceScope("ProcessActivity");
                _logger.LogInformation("Running ProcessActivity — preparing Service Bus message");

                TraceMetric("contoso.ProcessActivity.invoked", 1);

                var messagePayload = new MySbPayload
                {
                    SomeMessageId = Guid.NewGuid().ToString(),
                    SomeAction = "ProcessedData",
                    ATimestamp = DateTime.UtcNow
                };

                // Simulate failure (demo only)
                if (DateTime.Now.Millisecond % 2 == 1)
                    throw new InvalidOperationException("This is an intentionally thrown exception for demonstration.");

                // Automatically create envelope with telemetry context
                var envelope = EnvelopeContext<MySbPayload>.CreateWithContext(messagePayload);

                // Send message with enriched headers
                await _serviceBusEnvelopeSender.SendAsync(envelope, _sbOptions.QueueName);

                _logger.LogInformation(string.Format("Message sent to Service Bus queue: {0}", _sbOptions.QueueName));

            }, "ProcessActivity.Run");
        }
    }
}