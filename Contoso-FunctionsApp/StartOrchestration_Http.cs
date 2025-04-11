using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Contoso.Infrastructure.Context;
using Contoso.Utilities.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Contoso.FunctionsApp
{
    public class StartOrchestration_Http
    {
        private readonly IContextLogger _logger;
        private readonly IContextTracer _tracer;

        public StartOrchestration_Http(IContextLogger logger, IContextTracer tracer)
        {
            _logger = logger;
            this._tracer = tracer;
        }

        [FunctionName("StartOrchestration_Http")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        // [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        // [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient starter
        )
        {
            var headers = req.Headers;

            var ctx = new MyAppContext
            {
                ApplicationId = headers["x-app-id"],
                UserCode = headers["x-user-code"],
                OrchestrationId = Guid.NewGuid().ToString()
            };

            var tx = new DistributedTransactionContext
            {
                TransactionId = Guid.NewGuid().ToString(),
                CurrentStep = "HttpTrigger",
                Status = "Started"
            };

            ContextHolder<MyAppContext>.Current = ctx;
            ContextHolder<DistributedTransactionContext>.Current = tx;

            using var span = _tracer.StartSpan("StartOrchestration_Http");
            _logger.LogInfo("Starting orchestration from HTTP");


            var input = new OrchestrationInput
            {
                MyAppContext = ctx,
                DistributedTransactionContext = tx
            };

            var instanceId = await starter.StartNewAsync("MainOrchestrator", input);

            // string responseMessage = $"Orchestration started: {instanceId}";
            string responseMessage = $"Orchestration started: {ContextHolder<MyAppContext>.Current.OrchestrationId}";

            // notice here and in the implementation of LogInfo, how the `context` is retrieved for logging
            _logger.LogInfo(responseMessage);
            // _logger.LogInfo(responseMessage, ctx);

            return new OkObjectResult(responseMessage);

        }
    }
}

