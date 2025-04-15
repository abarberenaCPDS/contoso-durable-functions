using Contoso.Utilities.Context;

namespace Contoso.Infrastructure.Context
{
    // This Binder streamlines context setup by simplifying all boilerplate code   
    public static class TelemetryContextBinder
    {
        public static ITelemetryContext CreateFromHeaders(IDictionary<string, object> headers)
        {
            if (headers == null)
                throw new ArgumentNullException(nameof(headers));

            ITelemetryContext telemetryContext;

            var ctx = new MyAppContext
            {
                ApplicationId = headers.TryGetValue("app.ApplicationId", out var appId) ? appId?.ToString() : null,
                UserCode = headers.TryGetValue("app.UserCode", out var userCode) ? userCode?.ToString() : null,
                OrchestrationId = headers.TryGetValue("app.OrchestrationId", out var orchId) ? orchId?.ToString() : null
            };

            var tx = new DistributedTransactionContext
            {
                TransactionId = headers.TryGetValue("tx.TransactionId", out var txId) ? txId?.ToString() : null,
                CurrentStep = headers.TryGetValue("tx.CurrentStep", out var step) ? step?.ToString() : null,
                Status = headers.TryGetValue("tx.Status", out var status) ? status?.ToString() : null
            };

            telemetryContext = new OrchestrationInputContext
            {
                MyAppContext = ctx,
                DistributedTransactionContext = tx
            };

            return telemetryContext;
        }
    }
}
