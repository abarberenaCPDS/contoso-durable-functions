namespace Contoso.Infrastructure.Context
{
    public static class TransactionEnricher
    {
        public static void MarkStep(string stepName)
        {
            var ctx = ContextHolder<DistributedTransactionContext>.Current ?? new DistributedTransactionContext();
            ctx.CurrentStep = stepName;
            ctx.Status = "StepCompleted";
            ContextHolder<DistributedTransactionContext>.Current = ctx;
        }

        public static void MarkFailed(string stepName)
        {
            var ctx = ContextHolder<DistributedTransactionContext>.Current ?? new DistributedTransactionContext();
            ctx.CurrentStep = stepName;
            ctx.Status = "RolledBack";
            ContextHolder<DistributedTransactionContext>.Current = ctx;
        }
    }
}
