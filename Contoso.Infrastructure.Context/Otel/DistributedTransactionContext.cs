using System;
using System.Runtime.Serialization;

namespace Contoso.Infrastructure.Context
{
    [Serializable]
    [DataContract]
    public class DistributedTransactionContext
    {
        [DataMember]
        public string TransactionId { get; set; } = Guid.NewGuid().ToString();
        
        [DataMember]
        public string CurrentStep { get; set; }
        
        [DataMember]
        public string Status { get; set; } = "Started";

        public override string ToString() =>
            string.Format("TxId: {TransactionId}, Step: {CurrentStep}, Status: {Status}", TransactionId, CurrentStep, Status);
    }
}
