using System.Collections.Generic;
using Contoso.Infrastructure.Context;

namespace Contoso.Infrastructure.Core
{
    public class HeaderCarrierStrategy : IHeaderCarrierStrategy
    {
        public void Set<T>(IDictionary<string, object>? carrier, T value) where T : class
        {
            if (carrier != null && value != null)
            {
                Header<T>.Set(carrier, value);
            }
        }

        public T? Get<T>(IDictionary<string, object>? carrier) where T : class
        {
            if (carrier == null)
                return null;

            return Header<T>.Get(carrier);
        }
    }
}