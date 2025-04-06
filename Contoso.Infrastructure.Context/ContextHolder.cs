using System;
using System.Threading;

namespace Contoso.Infrastructure.Context
{
    public static class ContextHolder<T> where T : class
    {
        private static readonly AsyncLocal<T> _context = new();

        public static T Current
        {
            get => _context.Value;
            set => _context.Value = value;
        }
    }
}
