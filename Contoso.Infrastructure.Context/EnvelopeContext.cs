using Contoso.Utilities.Context;

namespace Contoso.Infrastructure.Context
{
    public class EnvelopeContext<T>
    {
        public T Payload { get; set; } = default!;
        public IDictionary<string, object> Headers { get; set; } = new Dictionary<string, object>();

        public static EnvelopeContext<T> CreateWithContext(T payload)
        {
            var envelope = new EnvelopeContext<T>
            {
                Payload = payload
            };

            var context = ContextHolder<ITelemetryContext>.Current;
            if (context != null)
            {
                foreach (var tag in context.GetTelemetryTags())
                {
                    envelope.Headers[tag.Key] = tag.Value!;
                }
            }

            return envelope;
        }
    }
}