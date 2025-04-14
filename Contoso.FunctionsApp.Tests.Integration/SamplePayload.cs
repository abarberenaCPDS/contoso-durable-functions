using Contoso.Infrastructure.Context;

namespace Contoso.FunctionsApp.Tests.Integration
{
    public static class SamplePayload
    {
        public static OrchestrationInputContext CreateTestInput()
        {
            return new OrchestrationInputContext
            {
                MyAppContext = new MyAppContext
                {
                    ApplicationId = "TestApp",
                    UserCode = "test-user",
                    OrchestrationId = Guid.NewGuid().ToString()
                },
                DistributedTransactionContext = new DistributedTransactionContext
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    CurrentStep = "TestStep",
                    Status = "Test"
                }
            };
        }
    }
}
