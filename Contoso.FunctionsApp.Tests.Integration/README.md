# 🧪 Contoso.FunctionsApp.Tests.Integration

## Purpose

This test project verifies the integration behavior of the `ProcessActivity` Azure Function by:

- ✅ Executing the function with a realistic `OrchestrationInput`
- ✅ Sending a message to a real or test Service Bus queue
- ✅ Asserting that the message was received
- ✅ Verifying that message headers contain propagated context:
  - `MyAppContext`
  - `DistributedTransactionContext`
- ✅ Capturing structured logs using a custom in-memory `FakeLogger`

---

## Test Flow

1. Initializes `ProcessActivity` with test configuration and logger
2. Generates a fake orchestration input with context
3. Invokes the activity function directly
4. Waits for the message on the target Service Bus queue
5. Asserts message body and header correctness
6. Optionally inspects logs for context-aware output

---

## Dependencies

- `xUnit` – test framework
- `Azure.Messaging.ServiceBus` – to send/receive messages
- `Microsoft.Extensions.Options` – to inject test config
- `FakeLogger` – test logger for `IContextLogger`

---

## Key Files

| File | Purpose |
|------|---------|
| `ProcessActivityTests.cs` | Main test for verifying Service Bus message and context |
| `SampleData.cs` | Creates dummy context input |
| `TestServiceBusOptions.cs` | Provides fake `IOptions<ServiceBusOptions>` |
| `FakeLogger.cs` | In-memory logger to capture structured logs |
| `appsettings.integration.json` | Holds test connection string and queue name |

---

## Notes

- Tests run live against a configured Service Bus queue
- Ensure the queue exists and is accessible before running
- Use a separate test namespace or cleanup after tests
