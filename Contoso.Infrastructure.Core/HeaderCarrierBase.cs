using System.Collections.Generic;
using Contoso.Infrastructure.Context;

namespace Contoso.Infrastructure.Core
{
    public abstract class HeaderCarrierBase<T> where T : class
    {
        protected virtual void AddHeader(IDictionary<string, object> dict, T header)
        {
            Header<T>.Set(dict, header);
        }

        protected virtual T GetHeader(IDictionary<string, object> dict)
        {
            return Header<T>.Get(dict);
        }
    }
}
