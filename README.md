# Contoso Durable Functions

A production-grade Azure Functions project built with .NET 8 (in-process), leveraging:
- Azure Durable Functions
- Structured telemetry with OpenTelemetry
- Centralized logging and exception handling
- Context-aware messaging and orchestration
- Application Insights and Datadog integration

## Project Structure

```
Contoso-FunctionsApp/
│   └── Durable and triggered functions
Contoso.Infrastructure.*
│   └── Messaging, context, headers, configuration
Contoso.Utilities.*
│   └── Telemetry pipeline, logging, error handling
```

## Features

- Durable orchestrators and activity functions
- Service Bus trigger and outbound messaging
- Ambient context propagation with `ContextHolder<T>`
- Centralized telemetry via `ITelemetryPipeline`
- Consistent logging via `IContextLogger`
- `FunctionWrapper` utility for error handling and spans
- OpenAPI annotations for HTTP functions
- Local development with secrets and fallback console logging

## Core Design Patterns

### FunctionWrapper

All functions are wrapped using `FunctionWrapper` or the `RunWithHandling` helper to provide:

- Centralized span creation
- Exception logging and recording
- Reduced boilerplate and improved consistency

```csharp
await RunWithHandling(async () =>
{
    // business logic
}, "ProcessActivity.Run");
```

### Context Propagation

Uses `ContextHolder<ITelemetryContext>.Current` to retain and flow metadata like:

- Application ID
- User code
- Transaction ID
- Orchestration step

These values are automatically serialized in outbound headers and enriched in telemetry.

## Local Development Setup

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Azure Functions Core Tools v4](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local)
- Azure Storage Emulator or Azurite
- Azure Service Bus Emulator
- Application Insights instrumentation key (optional)

### Run Locally

```bash
func start
```

## Telemetry

### Traced with:

- [OpenTelemetry SDK for .NET](https://opentelemetry.io/docs/instrumentation/net/)
- `ActivitySource`, `Meter`, and structured tags
- Logs, spans, metrics, and custom tags

### Export Targets:

- Azure Application Insights
- Datadog (via `TelemetryOptions`)
- Console (for local/dev)

## Deployment

This project is designed to deploy via:

- GitHub Actions
- Azure DevOps Pipelines
- Manual zip deployment with `func azure functionapp publish`


## Resources

- [Durable Functions Patterns](https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview)
- [OpenTelemetry .NET Docs](https://opentelemetry.io/docs/instrumentation/net/)
- [Azure Functions Best Practices](https://learn.microsoft.com/en-us/azure/azure-functions/functions-best-practices)