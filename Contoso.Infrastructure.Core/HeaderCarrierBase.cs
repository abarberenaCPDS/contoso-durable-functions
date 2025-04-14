using System.Collections.Generic;

namespace Contoso.Infrastructure.Core
{
    public abstract class HeaderCarrierBase<T> where T : class
    {
        protected readonly IHeaderCarrierStrategy _headerCarrier;

        protected HeaderCarrierBase(IHeaderCarrierStrategy headerCarrier)
        {
            _headerCarrier = headerCarrier;
        }

        protected virtual void AddHeader(IDictionary<string, object>? dict, T header)
        {
            _headerCarrier.Set(dict, header);
        }

        protected virtual T? GetHeader(IDictionary<string, object>? dict)
        {
            return _headerCarrier.Get<T>(dict);
        }
    }
}