using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureStorage
{
    public static class QueueStorageFunction
    {
        private static string _connectionString = "DefaultEndpointsProtocol=https;AccountName=storagejucal;AccountKey=2b9KE7grKEji4wHZIdyInZtLezaYvVgx/9R8bk1l509FxIpsvfLGsyATM/AZFkZwJSlhfMHE8GG3+AStlxSnbA==;EndpointSuffix=core.windows.net";

        private static readonly string _queueName = "orders";

        [FunctionName("ProcessQueueMessage")]
        public static async Task Run(
            [QueueTrigger("orders", Connection = "AzureWebJobsStorage")] string queueMessage,
            ILogger log)
        {
            try
            {
                log.LogInformation($"Received message: {queueMessage}");



                // Simulate processing the message 
                log.LogInformation("Processing the message...");

                // After processing message has been successfully processed
                log.LogInformation($"Message {queueMessage} processed successfully.");
            }
            catch (Exception ex)
            {
                // Log any errors that occur 
                log.LogError($"Error processing message {queueMessage}: {ex.Message}");
            }
        }
    }
}
