# 🧭 Contoso Durable Functions – Context + Logging Overview

## 🔧 Solution

- **Azure Durable Functions** (in-process model)
- Custom **context propagation** system using:
  - `MyAppContext`
  - `DistributedTransactionContext`
- Centralized, structured **logging system** via `ContextLogger`
- Built-in **OpenTelemetry tracing** to Datadog (via OTLP)

---

## 📦 Core Concepts

| Concept | Purpose |
|--------|---------|
| `MyAppContext` | Holds request metadata: `ApplicationId`, `UserCode`, `OrchestrationId` |
| `DistributedTransactionContext` | Tracks a logical transaction or workflow (step, status, rollback) |
| `ContextHolder<T>` | Stores the current context using `AsyncLocal<T>` so it flows across layers |
| `Header<T>` | Helper to set/get typed context values inside a dictionary (used between orchestration steps) |
| `ContextLogger` | Logs with structured context, supports App Insights, Datadog, and local console colors |
| `ContextAwareBase<T>` | Optional base class to simplify context setup per function or service class |

---

## 🚀 How It Works (Developer Flow)

1. **HTTP trigger** reads incoming headers and builds `MyAppContext` + `DistributedTransactionContext`.
2. These are passed as a DTO (`OrchestrationInput`) to the orchestration.
3. The orchestrator and activities **set context** using `ContextHolder<T>.Current`.
4. Functions call `ContextLogger.LogInformation(...)`, which logs the message and the current context.
5. Optional spans are created using `ActivityTracing.StartSpan(...)` and exported to Datadog.

#### Flow:
```
[HTTP Trigger]
  └─> StartOrchestration_Http (creates OrchestrationInput with headers)
       └─> MainOrchestrator
            └─> ProcessActivity
                 └─> Sends SB message with payload + propagated context headers
                      └─> HandleServiceBusMessage
                           └─> Reads payload, extracts context, logs + traces it

```
---


## 🔍 Developer Tips

- ✅ Use `ContextHolder<T>.Current` to **access context anywhere**
- ✅ Use `ContextLogger.LogInformation(...)` to **log with context**
- ✅ Use `OrchestrationInput` DTO to pass both `MyAppContext` and `DistributedTransactionContext`
- ❌ Avoid `Header<T>` unless you're passing raw `Dictionary<string, object>` between orchestrator and activity

---

## 🧪 Local Debugging Tips

- Set `UseConsoleColors = true` in `local.settings.json` to highlight logs
- Ensure `AZURE_FUNCTIONS_ENVIRONMENT=Development` for local behavior
- Use `DD_API_KEY` to push spans to Datadog via OTLP

---


## Usage

Set context via headers on HTTP or Service Bus:
  ```sh
  x-app-id:MyApp123 x-user-code:ABC123
  ```

Logs will contain:
  ```json
  "message" : "Starting orchestration", "context" : { "ApplicationId": "MyApp", "UserCode": "ABC123", "OrchestrationId": "auto-generated" }
  ```

  ## Testing

- See [Contoso.FunctionsApp.Tests.Integration/README.md](./Contoso.FunctionsApp.Tests.Integration/README.md)


## Resources

- https://github.com/Azure/azure-functions-core-tools/issues/3766
- https://www.jpatrickfulton.dev/blog/2023-07-08-fix-csharp-macos-debugging/
- https://github.com/Azure/azure-service-bus-emulator-installer