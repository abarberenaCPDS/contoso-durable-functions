using Contoso.Infrastructure.Context;
using System;

namespace Contoso.Utilities.Logging
{
    public interface IContextLogger
    {
        void LogInfo(string message, MyAppContext context = null);
        void LogWarning(string message, MyAppContext context = null);
        void LogError(Exception ex, string message = null, MyAppContext context = null);
    }
}
