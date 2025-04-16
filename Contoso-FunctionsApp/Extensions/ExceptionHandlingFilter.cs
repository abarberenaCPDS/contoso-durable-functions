using System;
using System.Threading;
using System.Threading.Tasks;
using Contoso.Infrastructure.Context;
using Contoso.Utilities.Context;
using Contoso.Utilities.Logging;
using Microsoft.Azure.WebJobs.Host;

namespace ContosoFunctionsApp.Extensions
{
    /// <summary>
    /// // IFunctionInvocationFilter is marked as [Obsolete]
    // https://github.com/azure/azure-webjobs-sdk/wiki/function-filters
    // https://github.com/Azure/azure-webjobs-sdk/blob/dev/src/Microsoft.Azure.WebJobs.Host/Filters/IFunctionInvocationFilter.cs
    /// </summary>
    [Obsolete("Do not use this filter...")]
    public class ExceptionHandlingFilter : IFunctionInvocationFilter
    {
        private readonly ITelemetryPipeline _telemetry;
        private readonly IContextLogger _logger;

        public ExceptionHandlingFilter(ITelemetryPipeline telemetry, IContextLogger logger)
        {
            _telemetry = telemetry;
            _logger = logger;
        }

        public Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
        {
            // Optional: Add pre-execution logic, such as context inspection
            return Task.CompletedTask;
        }

        public Task OnExecutedAsync(FunctionExecutedContext executedContext, CancellationToken cancellationToken)
        {
            var exception = executedContext.FunctionResult?.Exception;
            if (exception != null)
            {
                var context = ContextHolder<ITelemetryContext>.Current;
                _telemetry.RecordException(exception, context);
                _logger.LogError(exception, $"Exception thrown by function '{executedContext.FunctionName}'", context);
            }

            return Task.CompletedTask;
        }
    }
}

