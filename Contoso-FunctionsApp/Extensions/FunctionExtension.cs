using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contoso.Infrastructure.Context;
using Contoso.Infrastructure.Core;
using Contoso.Utilities.Context;
using Contoso.Utilities.Logging;

namespace ContosoFunctionsApp.Extensions
{
    public abstract class FunctionExtension : ContextAwareBase<ITelemetryContext>
    {
        protected readonly ITelemetryPipeline _telemetry;
        protected readonly IContextLogger _logger;

        protected FunctionExtension(IContextLogger logger, ITelemetryPipeline telemetry, IHeaderCarrierStrategy headerStrategy)
            : base(logger, headerStrategy)
        {
            _telemetry = telemetry;
            _logger = logger;
        }

        // Centralized handling helpers
        protected Task RunWithHandling(Func<Task> action, string operationName)
            => FunctionWrapper.HandleAsync(action, operationName, _telemetry, _logger, ContextHolder<ITelemetryContext>.Current);

        protected Task<TResult> RunWithHandling<TResult>(Func<Task<TResult>> action, string operationName)
            => FunctionWrapper.HandleAsync(action, operationName, _telemetry, _logger, ContextHolder<ITelemetryContext>.Current);

        // Centralized instrumentation helpers
        protected IDisposable TraceScope(string operation, ITelemetryContext? context = null)
            => _telemetry.BeginSpan(operation, context, out _);

        protected void TraceMetric(string name, long value, ITelemetryContext? context = null, params KeyValuePair<string, object?>[] tags)
            => _telemetry.RecordMetric(name, value, context, tags);

        protected void TraceError(Exception ex, ITelemetryContext? context = null)
            => _telemetry.RecordException(ex, context);
    }
}