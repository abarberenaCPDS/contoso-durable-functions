using System;
using System.Collections.Generic;
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

        protected IDisposable TraceScope(string operation, ITelemetryContext? context = null)
            => _telemetry.BeginSpan(operation, context, out _);

        protected void TraceMetric(string name, long value, ITelemetryContext? context = null, params KeyValuePair<string, object?>[] tags)
            => _telemetry.RecordMetric(name, value, context, tags);

        protected void TraceError(Exception ex, ITelemetryContext? context = null)
            => _telemetry.RecordException(ex, context);
    }
}