namespace Contoso.Infrastructure.Messaging
{
    public class MySbPayload
    {
        public string SomeMessageId { get; set; }
        public string SomeAction { get; set; }
        public DateTime ATimestamp { get; set; }
    }
}
