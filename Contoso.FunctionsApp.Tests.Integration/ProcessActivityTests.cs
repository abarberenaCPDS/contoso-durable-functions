// using Azure.Messaging.ServiceBus;
// using Contoso.FunctionsApp;
// using Contoso.Infrastructure.Messaging;
// using Contoso.Utilities.Logging;
// using Microsoft.Extensions.Configuration;
// using System.Text.Json;
// using Xunit;
// using Xunit.Abstractions;

// namespace Contoso.FunctionsApp.Tests.Integration
// {
//     public class ProcessActivityTests
//     {
//         private readonly string _queueName;
//         private readonly string _connectionString;

//         private readonly ITestOutputHelper _output;
//         private readonly FakeLogger logger;

//         public ProcessActivityTests(ITestOutputHelper output)
//         {
//             _output = output;
//             logger = new FakeLogger(); // Implement IContextLogger that stores output

//             var config = TestConfiguration.Build();
//             _queueName = config["ServiceBusQueueName"];
//             _connectionString = config["ServiceBus:ConnectionString"];
//         }

//         //[Fact]
//         //public async Task ProcessActivity_SendsMessageWithContextHeaders()
//         //{
//         //    // Arrange
//         //    var sbOptions = new TestSbOptions(_connectionString, _queueName);
//         //    var function = new ProcessActivity(logger, sbOptions);

//         //    var input = SamplePayload.CreateTestInput();

//         //    // Act

//         //    // Run ProcessActivity (which sets both contexts and logs)
//         //    await function.Run(input);

//         //    // Assert: read from queue...
//         //    var client = new ServiceBusClient(_connectionString);
//         //    var receiver = client.CreateReceiver(_queueName);
//         //    var message = await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(5));

//         //    MySbPayload mySbPayload = JsonSerializer.Deserialize<MySbPayload>(message.Body);



//         //    Assert.NotNull(message);
//         //    Assert.Equal("ProcessedData", mySbPayload?.SomeAction);
//         //    Assert.True(message.ApplicationProperties.ContainsKey("x-app-id"));
//         //    Assert.True(message.ApplicationProperties.ContainsKey("x-user-code"));

//         //    // ensure the correct format used in the FAKE logger:

//         //    //Assert.Contains(logger.InfoMessages, msg =>
//         //    //    msg.Contains("Running ProcessActivity — sending message to Service Bus")
//         //    //    );

//         //    // Assert both context and transaction info...
//         //    //Assert.Contains(logger.InfoMessages, msg =>
//         //        //msg.Contains("Running ProcessActivity — sending message to Service Bus") &&
//         //    //    msg.Contains("AppId=TestApp") &&
//         //    //    msg.Contains("TxId=") &&
//         //    //    msg.Contains("Step=TestStep")
//         //    //);

//         //    // Output all Info messages
//         //    foreach (var log in logger.InfoMessages)
//         //    {
//         //        _output.WriteLine($"[INFO] {log}");
//         //    }

//         //    // Output all Info messages
//         //    foreach (var log in logger.ErrorMessages)
//         //    {
//         //        _output.WriteLine($"[ERROR] {log}");
//         //    }

//         //    // Optional cleanup
//         //    await receiver.CompleteMessageAsync(message);
//         //}
//     }
// }
