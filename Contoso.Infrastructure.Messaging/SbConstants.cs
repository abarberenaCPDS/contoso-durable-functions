namespace Contoso.Infrastructure.Messaging
{
    public static class SbConstants
    {
        public static class QueueNames
        {
            // For use in [ServiceBusTrigger]
            public const string ProcessedItemsBinding = "%ServiceBusQueueName%";

            // // For use in config resolution (manual)
            public const string ProcessedItemsSetting = "ServiceBusQueueName";
        }

        public static class ConnectionStrings
        {
            public const string Primary = "ServiceBus:ConnectionString";
        }
    }
}