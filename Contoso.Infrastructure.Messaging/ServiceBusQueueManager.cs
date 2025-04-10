using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus.Administration;

namespace Contoso.Infrastructure.Messaging
{
    public static class ServiceBusQueueManager
    {
        ///// <summary>
        ///// Verifies that the specified queue exists; if not, it creates the queue.
        ///// </summary>
        ///// <param name="connectionString">The service bus Connection String.</param>
        ///// <param name="queueName">The name of queue to check if exists .</param>
        public static async Task VerifyQueueAsync(string connectionString, string queueName)
        {
            var adminClient = new ServiceBusAdministrationClient(connectionString);
            if (!await adminClient.QueueExistsAsync(queueName))
            {
                await adminClient.CreateQueueAsync(queueName);
                // Console.WriteLine($"Queue '{queueName}' created.");
            }
        }

        /// <summary>
        /// Purges the queue by deleting and then recreating it.
        /// Note: Azure Service Bus does not offer a native purge operation;
        /// deleting and recreating the queue is used here only for testing or maintenance.
        /// </summary>
        /// <param name="sbOptions">The service bus configuration options.</param>
        public static async Task PurgeQueueAsync(SbOptions sbOptions)
        {
            var adminClient = new ServiceBusAdministrationClient(sbOptions.ConnectionString);
            bool exists = await adminClient.QueueExistsAsync(sbOptions.QueueName);
            if (exists)
            {
                await adminClient.DeleteQueueAsync(sbOptions.QueueName);
                //Console.WriteLine($"Queue '{settings.QueueName}' deleted for purge.");
            }
            await adminClient.CreateQueueAsync(sbOptions.QueueName);
            //Console.WriteLine($"Queue '{settings.QueueName}' recreated.");
        }
    }
}