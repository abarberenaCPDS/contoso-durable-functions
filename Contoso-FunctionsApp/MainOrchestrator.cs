using System.Collections.Generic;
using System.Threading.Tasks;
using Contoso.Infrastructure.Context;
using Contoso.Infrastructure.Core;
using Contoso.Utilities.Logging;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Contoso.FunctionsApp
{
    public class MainOrchestrator : ContextAwareBase<MyAppContext>
    {
        public MainOrchestrator(IContextLogger logger,IContextTracer tracer) : base(logger, tracer) { }

        [FunctionName("MainOrchestrator")]
        public async Task Run([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var input = context.GetInput<OrchestrationInput>();

            // Load context into AsyncLocal
            ContextHolder<MyAppContext>.Current = input.MyAppContext;

            // Load distributed tx context
            ContextHolder<DistributedTransactionContext>.Current = input.DistributedTransactionContext;

            // using spans from OpenTelemetry
            // using var span = ActivityTracing.StartSpan("MainOrchestrator");
            // TransactionEnricher.MarkStep("MainOrchestrator");
            
            using var span = _tracer.StartSpan("MainOrchestrator");
            this._logger.LogInfo("Running MainOrchestrator");

            // Propagate both headers into the activity call
            // this is left for obviousness, there is no need to manually re-wrap it in a dictionary
            // and pass it again as OUTPUT, 
            var output = new Dictionary<string, object>
            {
                { nameof(MyAppContext), input.MyAppContext },
                { nameof(DistributedTransactionContext), input.DistributedTransactionContext }
            };
            // await context.CallActivityAsync("ProcessActivity", output);

            // BONUS: keep in mind, that you can now pass OrchestrationInput directly into the activity
            await context.CallActivityAsync("ProcessActivity", input);
            this._logger.LogInfo("Completed MainOrchestrator");
        }
    }
}