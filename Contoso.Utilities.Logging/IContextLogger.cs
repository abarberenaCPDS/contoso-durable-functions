using Contoso.Infrastructure.Context;
using Contoso.Utilities.Context;
using Microsoft.Extensions.Logging;

namespace Contoso.Utilities.Logging
{
    public interface IContextLogger
    {
        void LogInformation(string message, ITelemetryContext context = null);
        void LogWarning(string message, ITelemetryContext context = null);
        void LogError(Exception ex, string message = null, ITelemetryContext context = null);
    }
}
