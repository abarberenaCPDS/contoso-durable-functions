using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Contoso.Infrastructure.Context;
using Microsoft.Extensions.Options;

namespace Contoso.Infrastructure.Messaging
{
    public class ServiceBusEnvelopeSender : IServiceBusEnvelopeSender
    {
        private readonly SbOptions _settings;
        private readonly ServiceBusClient _client;
        private readonly ConcurrentDictionary<string, ServiceBusSender> _senders = new();

        public ServiceBusEnvelopeSender(IOptions<SbOptions> options)
        {
            _settings = options.Value;
            _client = new ServiceBusClient(_settings.ConnectionString);
        }

         public async Task SendAsync<T>(EnvelopeContext<T> envelope, string target)
        {
            var targetName = string.IsNullOrWhiteSpace(target)
                ? _settings.QueueName
                : target;

            var messageBody = JsonSerializer.Serialize(envelope.Payload);
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody))
            {
                MessageId = Guid.NewGuid().ToString(),
            };

            foreach (var kvp in envelope.Headers)
            {
                message.ApplicationProperties[kvp.Key] = kvp.Value;
            }

            var sender = _senders.GetOrAdd(targetName, _client.CreateSender);
            await sender.SendMessageAsync(message);
        }

        //private void AddContextHeaders(ServiceBusMessage message, object? context, string prefix)
        //{
        //    if (context is ITelemetryContext telemetryCtx)
        //    {
        //        foreach (var tag in telemetryCtx.GetTelemetryTags())
        //        {
        //            var key = tag.Key.StartsWith("x-") ? tag.Key : $"{prefix}-{tag.Key.ToLower()}";
        //            message.ApplicationProperties[key] = tag.Value;
        //        }
        //    }
        //    else if (context != null)
        //    {
        //        // Fallback: use reflection
        //        var props = context.GetType().GetProperties();
        //        foreach (var prop in props)
        //        {
        //            var key = $"{prefix}-{prop.Name.ToLower()}";
        //            var value = prop.GetValue(context);
        //            if (value != null)
        //            {
        //                message.ApplicationProperties[key] = value;
        //            }
        //        }
        //    }
        //}
    }
}