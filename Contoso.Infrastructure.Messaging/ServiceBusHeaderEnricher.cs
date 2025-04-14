using Azure.Messaging.ServiceBus;
using Contoso.Infrastructure.Context;
using Contoso.Utilities.Context;

namespace Contoso.Infrastructure.Messaging
{
    public static class ServiceBusHeaderEnricher
    {
        public static void InjectTelemetryHeaders(ServiceBusMessage message)
        {
            var context = ContextHolder<ITelemetryContext>.Current;
            if (context == null) return;

            foreach (var tag in context.GetTelemetryTags())
            {
                if (!string.IsNullOrWhiteSpace(tag.Key) && tag.Value is not null)
                {
                    message.ApplicationProperties[tag.Key] = tag.Value;
                }
            }
        }
    }
}