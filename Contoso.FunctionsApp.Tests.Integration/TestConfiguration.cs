using Microsoft.Extensions.Configuration;

namespace Contoso.FunctionsApp.Tests.Integration
{
    public static class TestConfiguration
    {
        public static IConfiguration Build()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.integration.json", optional: false)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}