using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Contoso.Infrastructure.Messaging;
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

        public async Task SendAsync<TPayload, TAppContext, TTxContext>(
            TPayload payload,
            TAppContext appContext,
            TTxContext txContext,
            string target)
        {
            var targetName = string.IsNullOrWhiteSpace(target)
                ? _settings.QueueName
                : target;


            // TODO: Design Note
            //// Ensure the queue exists (idempotent)
            ///   - Should this framework be in charge of creating the Queue if NOT exists?
            ///   - Consider having enough privileges, you may not be allowed to manage queues
            ///   - Consider Catching ServiceBus Exceptions
            /// Disabled for now because we're using the SB emulator 
            // await ServiceBusQueueManager.VerifyQueueAsync(_settings.ConnectionString, targetName);

            // Serialize payload
            var messageBody = JsonSerializer.Serialize(payload);
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody))
            {
                MessageId = Guid.NewGuid().ToString(),
                //Subject = messageLabel
            };

            // Attach context headers
            AddContextHeaders(message, appContext, "x-app");
            AddContextHeaders(message, txContext, "x-tx");

            // Get or create sender
            var sender = _senders.GetOrAdd(targetName, _client.CreateSender);
            await sender.SendMessageAsync(message);
        }

        private void AddContextHeaders<TContext>(ServiceBusMessage message, TContext context, string prefix)
        {
            if (context == null) return;

            var props = typeof(TContext).GetProperties();
            foreach (var prop in props)
            {
                var key = $"{prefix}-{prop.Name.ToLower()}";
                var value = prop.GetValue(context);
                if (value != null)
                {
                    message.ApplicationProperties[key] = value;
                }
            }
        }
    }
}