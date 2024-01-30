using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace ApprovalProcessFunction.Services
{
    public static class Approval
    {
        public static async Task ExecuteApprovalProcess(IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // Etapa 1: Esperar pela aprovação
            DateTime dueTime = context.CurrentUtcDateTime.AddMinutes(5);
            Task<bool> approvalTask = context.WaitForExternalEvent<bool>("ApprovalEvent");
            Task timeoutTask = context.CreateTimer(dueTime, CancellationToken.None);

            // Etapa 2: Aguarda pela conclusão de uma das tarefas.
            if (approvalTask == await Task.WhenAny(approvalTask, timeoutTask))
                outputs.Add("Pedido aprovado!");
            else
                outputs.Add("O pedido expirou. Aprovação não recebida a tempo.");
        }
    }
}

