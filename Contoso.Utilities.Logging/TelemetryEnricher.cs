using System.Diagnostics;
using Contoso.Utilities.Context;

namespace Contoso.Utilities.Logging
{
    public class TelemetryEnricher : ITelemetryEnricher
    {
        public void Enrich(Activity activity, ITelemetryContext context)
        {
            // This could include tenant, deployment region, etc.
            //activity.SetTag("app.region", Environment.GetEnvironmentVariable("REGION") ?? "unknown");

            foreach (var tag in context.GetTelemetryTags())
            {
                activity.SetTag(tag.Key, tag.Value);
            }
        }
    }
}