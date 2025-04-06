using System;
using System.Diagnostics;

namespace Contoso.Infrastructure.Context
{
    public static class ActivityTracing
    {
        private static readonly ActivitySource Source = new("Contoso.DurableApp");

        public static Activity StartSpan(string name)
        {
            var ctx = ContextHolder<MyAppContext>.Current;
            var tx = ContextHolder<DistributedTransactionContext>.Current;

            var activity = Source.StartActivity(name, ActivityKind.Internal);
            if (activity == null) return null;

            activity.SetTag("application.id", ctx?.ApplicationId);
            activity.SetTag("user.code", ctx?.UserCode);
            activity.SetTag("orchestration.id", ctx?.OrchestrationId);

            activity.SetTag("transaction.id", tx?.TransactionId);
            activity.SetTag("transaction.step", tx?.CurrentStep);
            activity.SetTag("transaction.status", tx?.Status);

            return activity;
        }
    }
}
