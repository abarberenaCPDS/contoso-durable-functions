using System.Diagnostics;
using Contoso.Infrastructure.Context;

namespace Contoso.Utilities.Logging
{
    public class ContextTracer : IContextTracer
    {
        private static readonly ActivitySource Source = new(TracingConstants.SourceName);

        public IDisposable StartSpan(string name)
        {
            var activity = Source.StartActivity(name, ActivityKind.Internal);

            if (activity != null)
            {
                var appCtx = ContextHolder<MyAppContext>.Current;
                var txCtx = ContextHolder<DistributedTransactionContext>.Current;

                EnrichWithContext(activity, appCtx, txCtx);

                // Set correlation ID, choose between Transaction Id or App Id, notice the Fallback pattern
                string correlationId = appCtx?.OrchestrationId ?? txCtx?.TransactionId ?? Guid.NewGuid().ToString();
                // string correlationId = txCtx?.TransactionId ?? appCtx?.OrchestrationId ?? Guid.NewGuid().ToString();
                activity.SetTag("correlation.id", correlationId);
            }

            return activity; // Activity implements IDisposable
        }

        public void EnrichWithContext(Activity activity, MyAppContext appContext, DistributedTransactionContext txContext)
        {
            if (activity == null) return;

            if (appContext != null)
            {
                activity.SetTag("application.id", appContext.ApplicationId);
                activity.SetTag("user.code", appContext.UserCode);
                activity.SetTag("orchestration.id", appContext.OrchestrationId);
            }

            if (txContext != null)
            {
                activity.SetTag("tx.id", txContext.TransactionId);
                activity.SetTag("tx.step", txContext.CurrentStep);
                activity.SetTag("tx.status", txContext.Status);
            }
        }

        public void AddEvent(string eventName, IDictionary<string, object> attributes = null)
        {
            var activity = Activity.Current;
            if (activity == null) return;

            activity.AddEvent(new ActivityEvent(eventName));

            if (attributes != null)
            {
                foreach (var kvp in attributes)
                {
                    activity.SetTag($"event.{eventName}.{kvp.Key}", kvp.Value);
                }
            }
        }
    }
}