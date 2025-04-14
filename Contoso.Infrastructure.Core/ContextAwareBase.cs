using Contoso.Infrastructure.Context;
using Contoso.Utilities.Logging;
using System.Collections.Generic;

namespace Contoso.Infrastructure.Core
{
    public abstract class ContextAwareBase<T> : HeaderCarrierBase<T> where T : class
    {
        protected readonly IContextLogger _logger;

        protected ContextAwareBase(IContextLogger logger, IHeaderCarrierStrategy headerStrategy)
            : base(headerStrategy)
        {
            _logger = logger;
        }

        protected void SetContextHeader(IDictionary<string, object>? dict, T context)
        {
            ContextHolder<T>.Current = context;
            AddHeader(dict, context);
        }

        protected T? GetContextHeader(IDictionary<string, object>? dict)
        {
            return GetHeader(dict);
        }
    }
}
