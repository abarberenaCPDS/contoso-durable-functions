using DurableTask.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.DurableTask;
using System;

namespace Contoso.Infrastructure.Core
{
    public static class RetryPolicy
    {
        public static RetryOptions Standard => new(
            firstRetryInterval: TimeSpan.FromMinutes(1),
            maxNumberOfAttempts: 3)
        {
            BackoffCoefficient = 1.5,
            MaxRetryInterval = TimeSpan.FromMinutes(5),
            Handle = ex => !(ex is InvalidOperationException) // only retry on transient
        };
    }
}
