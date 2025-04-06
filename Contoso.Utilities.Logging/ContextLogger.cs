using Contoso.Infrastructure.Context;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace Contoso.Utilities.Logging
{
    public class ContextLogger : IContextLogger
    {
        private readonly ILogger<ContextLogger> _logger;
        private readonly LoggingOptions _options;

        public ContextLogger(ILogger<ContextLogger> logger, IOptions<LoggingOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public void LogInfo(string message, MyAppContext context = null)
        {
            context ??= ContextHolder<MyAppContext>.Current;

            if (_options.UseConsoleColors)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"MyContextLogger ==> [Info] {message} | {context}");
                Console.ResetColor();
            }

            _logger.LogInformation("MyContextLogger ==> {Message} | {@Context}", message, context);
        }

        public void LogWarning(string message, MyAppContext context = null)
        {
            context ??= ContextHolder<MyAppContext>.Current;

            if (_options.UseConsoleColors)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"MyContextLogger ==> [Warn] {message} | {context}");
                Console.ResetColor();
            }

            _logger.LogWarning("MyContextLogger ==> {Message} | {@Context}", message, context);
        }

        public void LogError(Exception ex, string message = null, MyAppContext context = null)
        {
            context ??= ContextHolder<MyAppContext>.Current;

            if (_options.UseConsoleColors)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"MyContextLogger ==> [Error] {message ?? ex.Message} | {context}");
                Console.ResetColor();
            }

            _logger.LogError(ex, "MyContextLogger ==> {Message} | {@Context}", message ?? ex.Message, context);
        }
    }
}
