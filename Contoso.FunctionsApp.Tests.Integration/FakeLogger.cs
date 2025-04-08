using System;
using System.Collections.Generic;
using System.Text.Json;
using Contoso.Infrastructure.Context;
using Contoso.Utilities.Logging;

namespace Contoso.FunctionsApp.Tests.Integration
{
    public class FakeLogger : IContextLogger
    {
        public List<string> InfoMessages { get; } = new();
        public List<string> WarningMessages { get; } = new();
        public List<string> ErrorMessages { get; } = new();
        public List<(string Message, Exception Exception)> Exceptions { get; } = new();

        public void LogInfo(string message, MyAppContext context = null)
        {
            InfoMessages.Add(Format(message, context));
        }

        public void LogWarning(string message, MyAppContext context = null)
        {
            WarningMessages.Add(Format(message, context));
        }

        public void LogError(Exception ex, string message = null, MyAppContext context = null)
        {
            var combined = $"{message ?? ex.Message} | Context: {FormatContext(context)} | Transaction: {FormatTransaction()}";
            ErrorMessages.Add(combined);
            Exceptions.Add((combined, ex));
        }

        private string Format(string message, MyAppContext context)
        {
            // this is the default format
            return $"{message} | Context: {FormatContext(context)} | Transaction: {FormatTransaction()}";

            // For this Fake logger, you could also do JSON-Style Log Formatting
            // This is recommended for structured logs or exporting to test reports.
            //var tx = ContextHolder<DistributedTransactionContext>.Current;
            //return JsonSerializer.Serialize(new
            //{
            //    message,
            //    context,
            //    transaction = tx
            //});
        }

        private string FormatContext(MyAppContext context)
        {
            return context == null
                ? "null"
                : $"AppId={context.ApplicationId}, User={context.UserCode}, Orchestration={context.OrchestrationId}";
        }

        private string FormatTransaction()
        {
            var tx = ContextHolder<DistributedTransactionContext>.Current;
            return tx == null
                ? "null"
                : $"TxId={tx.TransactionId}, Step={tx.CurrentStep}, Status={tx.Status}";
        }
    }
}
