using System;

namespace Contoso.Infrastructure.Context
{
    public class DistributedTransactionContext
    {
        public string TransactionId { get; set; } = Guid.NewGuid().ToString();
        public string CurrentStep { get; set; }
        public string Status { get; set; } = "Started";

        public override string ToString() =>
            $"TxId: {TransactionId}, Step: {CurrentStep}, Status: {Status}";
    }
}
