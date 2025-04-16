using System;
using System.Threading.Tasks;
using Contoso.Infrastructure.Context;
using Contoso.Utilities.Context;
using Contoso.Utilities.Logging;
using Microsoft.AspNetCore.Mvc;

namespace ContosoFunctionsApp.Extensions
{
    public static class FunctionWrapper
    {
        /// <summary>
        /// Executes a function asynchronously with centralized telemetry and exception handling.
        /// This version is intended for operations that do not return a value (e.g., Task, void logic)
        /// </summary>
        /// <param name="action">The asynchronous function body you want to run</param>
        /// <param name="operationName">Name used in logs and telemetry (e.g. "ProcessActivity.Run")</param>
        /// <param name="_telemetry">Injected ITelemetryPipeline, starts spans, records exceptions</param>
        /// <param name="_logger">Injected IContextLogger, writes structured logs</param>
        /// <param name="ContextHolder<ITelemetryContext>.Current">Captures current logical context (correlation ID, user, etc.)</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task HandleAsync(
            Func<Task> action,
            string operationName,
            ITelemetryPipeline telemetry,
            IContextLogger logger,
            ITelemetryContext? context = null)
        {
            context ??= ContextHolder<ITelemetryContext>.Current;

            using var span = telemetry.BeginSpan(operationName, context, out _);
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                telemetry.RecordException(ex, context);
                logger.LogError(ex, $"Unhandled exception in {operationName}", context);
                throw;
            }
        }

        /// <summary>
        /// Executes a function asynchronously with centralized telemetry and exception handling.
        /// This version is intended for operations that return a result (Task&lt;TResult&gt;).
        /// </summary>
        /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
        /// <param name="action">The asynchronous function body you want to run</param>
        /// <param name="operationName">Name used in logs and telemetry (e.g. "ProcessActivity.Run")</param>
        /// <returns>A task that yields the result of the operation.</returns>
        public static async Task<TResult> HandleAsync<TResult>(
            Func<Task<TResult>> action,
            string operationName,
            ITelemetryPipeline telemetry,
            IContextLogger logger,
            ITelemetryContext? context = null)
        {
            context ??= ContextHolder<ITelemetryContext>.Current;

            using var span = telemetry.BeginSpan(operationName, context, out _);
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                telemetry.RecordException(ex, context);
                logger.LogError(ex, $"Unhandled exception in {operationName}", context);
                throw;
            }
        }

        /// <summary>
        /// Executes an asynchronous HTTP operation with telemetry and error handling.
        /// Converts unexpected exceptions into HTTP 500 responses.
        /// </summary>
        /// <param name="action">The asynchronous operation that returns an IActionResult.</param>
        /// <param name="operationName">A name for the operation used in telemetry and logs.</param>
        /// <param name="telemetry">The telemetry pipeline used to record spans and exceptions.</param>
        /// <param name="logger">The context-aware logger used for error logging.</param>
        /// <param name="context">Optional telemetry context; if null, ContextHolder is used.</param>
        /// <returns>The result of the operation or a 500 response on unhandled error.</returns>
        public static async Task<IActionResult> HandleAsync(
            Func<Task<IActionResult>> action,
            string operationName,
            ITelemetryPipeline telemetry,
            IContextLogger logger,
            ITelemetryContext? context = null)
        {
            context ??= ContextHolder<ITelemetryContext>.Current;

            using var span = telemetry.BeginSpan(operationName, context, out _);
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                telemetry.RecordException(ex, context);
                logger.LogError(ex, $"Unhandled exception in {operationName}", context);
                return new ObjectResult("An unexpected error occurred.") { StatusCode = 500 };
            }
        }
    }
}