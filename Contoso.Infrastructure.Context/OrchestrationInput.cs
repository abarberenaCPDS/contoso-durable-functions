namespace Contoso.Infrastructure.Context
{
    public class OrchestrationInput
    {
        public MyAppContext MyAppContext { get; set; }
        public DistributedTransactionContext DistributedTransactionContext { get; set; }
    }
}
