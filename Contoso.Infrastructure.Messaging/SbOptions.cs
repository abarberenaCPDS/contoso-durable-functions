namespace Contoso.Infrastructure.Messaging
{
    public class SbOptions
    {
        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
        public string TopicName { get; set; }
        public string SubscriptionName { get; set; }
    }
}