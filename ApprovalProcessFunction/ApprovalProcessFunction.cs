using System.Threading.Tasks;
using ApprovalProcessFunction.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace ApprovalProcessFunction
{
    public static class ApprovalProcessFunction
    {
        [FunctionName("ApprovalProcess_HttpStart")]
        public static async Task<IActionResult> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
        {
            string instanceId = await starter.StartNewAsync("ApprovalProcess", null);

            log.LogInformation($"Iniciada instância de orquestração com ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName("ApprovalProcess")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context) =>
            await Approval.ExecuteApprovalProcess(context);
    }
}
