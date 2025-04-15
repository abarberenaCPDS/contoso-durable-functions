using System.Text;
using System.Transactions;
using Contoso.Utilities.Context;

namespace Contoso.Infrastructure.Context
{
    public class OrchestrationInputContext : ITelemetryContext
    {
        public MyAppContext MyAppContext { get; set; } = new();
        public DistributedTransactionContext DistributedTransactionContext { get; set; } = new();

        // Static Factory to OrchestrationInputContext
        public static OrchestrationInputContext CreateWithBinding(
            MyAppContext myAppContext,
            DistributedTransactionContext transactionContext)
        {
            var orchestrationContext = new OrchestrationInputContext
            {
                MyAppContext = myAppContext,
                DistributedTransactionContext = transactionContext
            };

            ContextHolder<ITelemetryContext>.Current = orchestrationContext;
            return orchestrationContext;
        }

        public IEnumerable<KeyValuePair<string, object?>> GetTelemetryTags()
        {
            yield return new("app.ApplicationId", MyAppContext.ApplicationId);
            yield return new("app.UserCode", MyAppContext.UserCode);
            yield return new("app.OrchestrationId", MyAppContext.OrchestrationId);

            yield return new("tx.TransactionId", DistributedTransactionContext.TransactionId);
            yield return new("tx.CurrentStep", DistributedTransactionContext.CurrentStep);
            yield return new("tx.Status", DistributedTransactionContext.Status);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var tag in this.GetTelemetryTags())
            {
                sb.AppendFormat(" {0}: {1}", tag.Key, tag.Value);
            }
            return sb.ToString();
        }
    }
}
