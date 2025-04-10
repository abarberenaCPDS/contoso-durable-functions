using System.Threading.Tasks;
using Contoso.Infrastructure.Context;

namespace Contoso.Infrastructure.Messaging
{
    public interface IServiceBusEnvelopeSender
    {
        Task SendAsync<TPayload, TAppContext, TDistribuitedTxContext>(
            TPayload payload,
            TAppContext applicationContext,
            TDistribuitedTxContext distributedTransactionContext,
            string queueName = null);
    }
}