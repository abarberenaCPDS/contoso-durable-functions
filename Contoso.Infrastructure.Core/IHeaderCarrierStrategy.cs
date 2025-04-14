using System.Collections.Generic;

namespace Contoso.Infrastructure.Core
{
    public interface IHeaderCarrierStrategy
    {
        void Set<T>(IDictionary<string, object>? carrier, T value) where T : class;
        T? Get<T>(IDictionary<string, object>? carrier) where T : class;
    }
}