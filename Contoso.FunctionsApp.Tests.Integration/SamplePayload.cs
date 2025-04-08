using Contoso.Infrastructure.Context;

namespace Contoso.FunctionsApp.Tests.Integration
{
    public static class SamplePayload
    {
        public static OrchestrationInput CreateTestInput()
        {
            return new OrchestrationInput
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
