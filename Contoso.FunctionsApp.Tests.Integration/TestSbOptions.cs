using Contoso.Infrastructure.Messaging;
using Microsoft.Extensions.Options;

namespace Contoso.FunctionsApp.Tests.Integration
{
    public class TestSbOptions : IOptions<SbOptions>
    {
        public SbOptions Value { get; }

        public TestSbOptions(string connectionString, string queueName)
        {
            Value = new SbOptions
            {
                ConnectionString = connectionString,
                QueueName = queueName
            };
        }
    }
}
