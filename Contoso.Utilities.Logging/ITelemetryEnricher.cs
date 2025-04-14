using System.Diagnostics;
using Contoso.Utilities.Context;

namespace Contoso.Utilities.Logging
{
    public interface ITelemetryEnricher
    {
        void Enrich(Activity activity, ITelemetryContext context);
    }
}

