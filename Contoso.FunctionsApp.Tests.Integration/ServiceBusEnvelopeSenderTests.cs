//using System;
//using Azure.Messaging.ServiceBus;
//using Contoso.Infrastructure.Context;
//using Contoso.Infrastructure.Messaging;
//using Contoso.Utilities.Context;
//using Microsoft.Extensions.Options;

//namespace Contoso.FunctionsApp.Tests.Integration
//{
//    public class ServiceBusEnvelopeSenderTests
//    {
//        [Fact]
//        public async Task SendAsync_Should_Inject_TelemetryContext_Headers()
//        {
//            // Arrange
//            var payload = new { Event = "SampleEvent", Timestamp = DateTime.UtcNow };
//            var context = new TestTelemetryContext();

//            ContextHolder<ITelemetryContext>.Current = context;

//            var options = Options.Create(new SbOptions
//            {
//                ConnectionString = "UseDevelopmentStorage=true", // Fake or test double
//                QueueName = "test-queue"
//            });

//            var sender = new TestableServiceBusEnvelopeSender(options);

//            // Act
//            await sender.SendAsync(payload, context, null, "test-queue");

//            // Assert
//            var sentMessage = sender.LastSentMessage;

//            Assert.NotNull(sentMessage);
//            Assert.Contains("x-app-id", sentMessage.ApplicationProperties.Keys);
//            Assert.Contains("x-correlation-id", sentMessage.ApplicationProperties.Keys);
//        }

//        private class TestableServiceBusEnvelopeSender : ServiceBusEnvelopeSender
//        {
//            public ServiceBusMessage? LastSentMessage { get; private set; }

//            public TestableServiceBusEnvelopeSender(IOptions<SbOptions> options)
//                : base(options)
//            { }

//            protected override async Task SendMessageInternalAsync(string queueName, ServiceBusMessage message)
//            {
//                LastSentMessage = message;
//                await Task.CompletedTask;
//            }
//        }

//        private class TestTelemetryContext : ITelemetryContext
//        {
//            public IEnumerable<KeyValuePair<string, object?>> GetTelemetryTags()
//            {
//                yield return new("x-app-id", "App123");
//                yield return new("x-correlation-id", "corr-abc");
//            }
//        }
//    }
//}

