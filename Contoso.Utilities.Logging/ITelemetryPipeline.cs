using System.Diagnostics;
using Contoso.Utilities.Context;

namespace Contoso.Utilities.Logging
{
    public interface ITelemetryPipeline
    {
        public IDisposable BeginSpan(string name, ITelemetryContext? context, out Activity? activity);
        public void RecordMetric(string name, long value, ITelemetryContext? context, params KeyValuePair<string, object?>[] tags);
        public void RecordException(Exception ex, ITelemetryContext? context);
    }
}