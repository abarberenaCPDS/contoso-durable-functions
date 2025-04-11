using Contoso.Infrastructure.Context;
using Contoso.Utilities.Logging;
using System.Collections.Generic;

namespace Contoso.Infrastructure.Core
{
    public abstract class ContextAwareBase<T> : HeaderCarrierBase<T> where T : class
    {
        protected readonly IContextLogger _logger;
        protected readonly IContextTracer _tracer;


        protected ContextAwareBase(IContextLogger logger, IContextTracer tracer)
        {
            _logger = logger;
            _tracer = tracer;
        }

        protected void SetContextHeader(IDictionary<string, object> dict, T ctx)
        {
            ContextHolder<T>.Current = ctx;
            AddHeader(dict, ctx);
        }
    }
}
