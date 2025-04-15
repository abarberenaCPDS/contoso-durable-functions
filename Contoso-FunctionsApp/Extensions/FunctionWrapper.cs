using System;
using System.Threading.Tasks;
using Contoso.Utilities.Context;
using Contoso.Utilities.Logging;
using Microsoft.AspNetCore.Mvc;

namespace ContosoFunctionsApp.Extensions
{
    public static class FunctionWrapper
    {
        public static async Task HandleAsync(
            Func<Task> action,
            string operationName,
            ITelemetryPipeline telemetry,
            IContextLogger logger,
            ITelemetryContext? context = null)
        {
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

        public static async Task<TResult> HandleAsync<TResult>(
            Func<Task<TResult>> action,
            string operationName,
            ITelemetryPipeline telemetry,
            IContextLogger logger,
            ITelemetryContext? context = null)
        {
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

        public static async Task<IActionResult> HandleAsync(
            Func<Task<IActionResult>> action,
            string operationName,
            ITelemetryPipeline telemetry,
            IContextLogger logger,
            ITelemetryContext? context = null)
        {
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