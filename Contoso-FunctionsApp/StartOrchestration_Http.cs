using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Contoso.Infrastructure.Context;
using Contoso.Infrastructure.Core;
using Contoso.Utilities.Context;
using Contoso.Utilities.Logging;
using ContosoFunctionsApp.Extensions;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Contoso.FunctionsApp
{
    public class StartOrchestration_Http : FunctionExtension
    {

        public StartOrchestration_Http(
            IContextLogger logger,
            ITelemetryPipeline telemetryPipeline,
            IHeaderCarrierStrategy headerStrategy)
            : base(logger, telemetryPipeline, headerStrategy)
        { }

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

            var orchestrationContext = OrchestrationInputContext.CreateWithBinding(ctx, tx);

            return await FunctionWrapper.HandleAsync(
                async () =>
                {
                    var headerCarrier = new Dictionary<string, object>();
                    SetContextHeader(headerCarrier, orchestrationContext);

                    using var span = TraceScope("StartOrchestration_Http.Run"); // context is now optional
                    _logger.LogInformation("Starting orchestration from HTTP");

                    var instanceId = await starter.StartNewAsync("MainOrchestrator", orchestrationContext);
                    TraceMetric("contoso.StartOrchestration_Http.Run", 1);

                    string responseMessage = $"Orchestration started: {orchestrationContext.MyAppContext.OrchestrationId}";

                    // notice here and in the implementation of LogInfo, how the `context` is retrieved for logging
                    _logger.LogInformation(responseMessage);
                    // _logger.LogInfo(responseMessage, ctx);

                    return new OkObjectResult(responseMessage);
                },
                "StartOrchestration_Http.Run",
                _telemetry,
                _logger,
                orchestrationContext
            );

        }
    }
}