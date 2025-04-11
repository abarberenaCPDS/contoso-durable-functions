using System;
using System.Diagnostics;
using Contoso.Infrastructure.Context;

namespace Contoso.Utilities.Logging
{
    public interface IContextTracer
    {
        IDisposable StartSpan(string name);
        void EnrichWithContext(Activity span, MyAppContext appContext, DistributedTransactionContext txContext);
    }
}