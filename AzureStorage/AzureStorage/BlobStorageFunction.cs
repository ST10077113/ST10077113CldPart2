using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs;

namespace AzureStorage
{
    public static class BlobStorageFunction
    {
        private static string _connectionString = "DefaultEndpointsProtocol=https;AccountName=storagejucal;AccountKey=2b9KE7grKEji4wHZIdyInZtLezaYvVgx/9R8bk1l509FxIpsvfLGsyATM/AZFkZwJSlhfMHE8GG3+AStlxSnbA==;EndpointSuffix=core.windows.net";

        private static readonly string _containerName = "uploads";

        [FunctionName("UploadToBlob")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Blob("uploads/{name}", FileAccess.Write, Connection = "AzureWebJobsStorage")] BlobClient blobClient,
            ILogger log)
        {
            string name = req.Query["name"]; // Get the file name from the query

            using (var stream = req.Body)
            {
                await blobClient.UploadAsync(stream, true);
            }

            return new OkObjectResult($"File {name} uploaded successfully.");
        }
    }
}
