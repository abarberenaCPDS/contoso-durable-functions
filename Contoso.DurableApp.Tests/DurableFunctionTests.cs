using System;
using Contoso.Infrastructure.Context;
using Contoso.Utilities.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Contoso.DurableApp.Tests
{
    public class DurableFunctionTests
    {
        [Fact]
        public void ContextHolder_StoresAndRetrievesContext()
        {
            var ctx = new MyAppContext
            {
                ApplicationId = "UnitTest",
                UserCode = "Tester",
                OrchestrationId = "orch-001"
            };

            ContextHolder.Current = ctx;
            var retrieved = ContextHolder.Current;

            Assert.Equal("UnitTest", retrieved.ApplicationId);
            Assert.Equal("Tester", retrieved.UserCode);
            Assert.Equal("orch-001", retrieved.OrchestrationId);
        }

        [Fact]
        public void Logger_Logs_With_Context()
        {
            var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<ContextLogger>>();
            var options = Microsoft.Extensions.Options.Options.Create(new LoggingOptions());
            var logger = new ContextLogger(mockLogger.Object, options);

            var ctx = new MyAppContext { ApplicationId = "TestApp", UserCode = "Dev" };
            ContextHolder.Current = ctx;

            logger.LogInfo("Unit test message", ctx);

            mockLogger.Verify(
                x => x.Log(
                    It.IsAny<Microsoft.Extensions.Logging.LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("TestApp")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ),
                Times.AtLeastOnce);
        }
    }
}
