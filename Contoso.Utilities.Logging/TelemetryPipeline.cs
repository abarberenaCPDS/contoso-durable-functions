using System.Diagnostics;
using System.Diagnostics.Metrics;
using Contoso.Infrastructure.Context;
using Contoso.Utilities.Context;

namespace Contoso.Utilities.Logging
{
    public class TelemetryPipeline : ITelemetryPipeline
    {
        private static readonly ActivitySource ActivitySource = new(TracingConstants.ServiceName);
        private static readonly Meter Meter = new(TracingConstants.ServiceName);

        private readonly ITelemetryEnricher _enricher;

        public TelemetryPipeline(ITelemetryEnricher enricher)
        {
            _enricher = enricher;
        }

        public IDisposable BeginSpan(string name, ITelemetryContext? context, out Activity? activity)
        {
            context ??= ContextHolder<ITelemetryContext>.Current;

            activity = ActivitySource.StartActivity(name, ActivityKind.Internal);

            if (activity != null && context != null)
            {
                _enricher.Enrich(activity, context);
            }

            return activity ?? Disposable.Empty;
        }

        public void RecordMetric(string name, long value, ITelemetryContext? context, params KeyValuePair<string, object?>[] tags)
        {
            context ??= ContextHolder<ITelemetryContext>.Current;

            var counter = Meter.CreateCounter<long>(name);

            if (context != null)
            {
                var allTags = tags.Concat(context.GetTelemetryTags());
                counter.Add(value, allTags.ToArray());
            }
            else
            {
                counter.Add(value, tags);
            }
        }

        public void RecordException(Exception ex, ITelemetryContext? context)
        {
            context ??= ContextHolder<ITelemetryContext>.Current;

            var activity = Activity.Current;
            if (activity != null)
            {
                activity.SetStatus(ActivityStatusCode.Error, ex.Message);
                activity.SetTag("exception.message", ex.Message);
                activity.SetTag("exception.stacktrace", ex.StackTrace);

                if (context != null)
                {
                    _enricher.Enrich(activity, context);
                }
            }
        }

        private class Disposable : IDisposable
        {
            public static readonly IDisposable Empty = new Disposable();
            public void Dispose() { }
        }
    }
}