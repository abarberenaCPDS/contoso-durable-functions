using System.Collections.Generic;
using System.Threading.Tasks;
using Contoso.Infrastructure.Context;
using Contoso.Infrastructure.Core;
using Contoso.Utilities.Context;
using Contoso.Utilities.Logging;
using ContosoFunctionsApp.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Contoso.FunctionsApp
{
    public class MainOrchestrator : FunctionExtension
    {
        public MainOrchestrator(IContextLogger logger, ITelemetryPipeline telemetry, IHeaderCarrierStrategy headerStrategy)
            : base(logger, telemetry, headerStrategy)
        {
        }

        [FunctionName("MainOrchestrator")]
        public async Task Run([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            await RunWithHandling(async () =>
            {

                var input = context.GetInput<OrchestrationInputContext>();
                input.DistributedTransactionContext.CurrentStep = "MainOrchestrator";
                ContextHolder<ITelemetryContext>.Current = input;

                _logger.LogInformation("Running orchestrator");
                await context.CallActivityAsync("ProcessActivity", input);
                _logger.LogInformation("Completed MainOrchestrator");

            }, "MainOrchestrator.Run");
        }
    }
}