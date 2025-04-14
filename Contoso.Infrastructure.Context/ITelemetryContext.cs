using System;
namespace Contoso.Utilities.Context
{
    public interface ITelemetryContext
    {
        IEnumerable<KeyValuePair<string, object?>> GetTelemetryTags();
    }
}

