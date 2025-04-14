using System;
using System.Collections.Generic;
using Contoso.Utilities.Context;

namespace Contoso.Infrastructure.Context
{
    // Optional Helper
    // If you want to streamline context setup, this helper can simplify boilerplate in all function triggers.
    
    public static class ContextSetupHelper
    {
        public static void SetFromHeaders(IDictionary<string, string> headers)
        {
            var ctx = new MyAppContext
            {
                ApplicationId = headers.TryGetValue("x-app-id", out var appId) ? appId : null,
                UserCode = headers.TryGetValue("x-user-code", out var userCode) ? userCode : null,
                OrchestrationId = headers.TryGetValue("x-orch-id", out var orchId) ? orchId : Guid.NewGuid().ToString()
            };

            var tx = new DistributedTransactionContext
            {
                TransactionId = headers.TryGetValue("x-tx-id", out var txid) ? txid : Guid.NewGuid().ToString(),
                CurrentStep = headers.TryGetValue("x-step", out var step) ? step : "unknown",
                Status = "Started"
            };

            // TODO: refactor this later
            ContextHolder<MyAppContext>.Current = ctx;
            ContextHolder<DistributedTransactionContext>.Current = tx;

            ContextHolder<OrchestrationInputContext>.Current.MyAppContext = ctx;
            ContextHolder<OrchestrationInputContext>.Current.DistributedTransactionContext = tx;
        }
    }
}
