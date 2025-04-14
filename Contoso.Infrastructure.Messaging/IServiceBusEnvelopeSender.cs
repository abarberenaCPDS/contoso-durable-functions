using Contoso.Infrastructure.Context;

namespace Contoso.Infrastructure.Messaging
{
    public interface IServiceBusEnvelopeSender
    {
        Task SendAsync<TPayload>(
            EnvelopeContext<TPayload> envelope,
            string target);
    }
}