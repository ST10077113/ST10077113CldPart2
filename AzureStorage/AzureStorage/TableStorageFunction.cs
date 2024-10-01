using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Data.Tables;
using Azure;

namespace AzureStorage
{
    public class TableStorageFunction
    {
        private static readonly TableClient _tableClient;


        public TableStorageFunction(TableClient _tableClient)
        {
            // Fetch the connection string from Azure Function App settings
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var serviceClient = new TableServiceClient(connectionString);
            _tableClient = serviceClient.GetTableClient("AddProduct");

            // Ensure the table exists
            _tableClient.CreateIfNotExists();
        }

        // Function to store data in Azure Table Storage
        [FunctionName("StoreToTable")]
        public static async Task<IActionResult> StoreToTableAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Storing data to Azure Table Storage...");

            // Read and parse request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<TableEntity>(requestBody);

            // Check if PartitionKey and RowKey are provided
            if (string.IsNullOrWhiteSpace(data.PartitionKey) || string.IsNullOrWhiteSpace(data.RowKey))
            {
                return new BadRequestObjectResult("PartitionKey and RowKey are required.");
            }

            // Add entity 
            try
            {
                await _tableClient.AddEntityAsync(data);
                return new OkObjectResult("Data stored successfully.");
            }
            catch (RequestFailedException ex)
            {
                log.LogError($"Error adding entity to table storage: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
