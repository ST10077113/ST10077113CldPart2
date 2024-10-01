using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Files.Shares;
using Azure;

namespace AzureStorage
{
    public static class FileShareFunction
    {
        private static string _connectionString = "DefaultEndpointsProtocol=https;AccountName=storagejucal;AccountKey=2b9KE7grKEji4wHZIdyInZtLezaYvVgx/9R8bk1l509FxIpsvfLGsyATM/AZFkZwJSlhfMHE8GG3+AStlxSnbA==;EndpointSuffix=core.windows.net";
        private static string _fileShareName = "soccerbootshare";

        [FunctionName("UploadFile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "uploadfile")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Processing file upload request.");

            // Read the file stream and directory name from the request
            var directoryName = req.Form["directoryName"];
            var file = req.Form.Files[0];

            if (file == null || file.Length == 0)
            {
                return new BadRequestObjectResult("No file uploaded.");
            }

            // Initialize the Azure File Share Service
            var serviceClient = new ShareServiceClient(_connectionString);
            var shareClient = serviceClient.GetShareClient(_fileShareName);
            var directoryClient = shareClient.GetDirectoryClient(directoryName);
            await directoryClient.CreateIfNotExistsAsync();

            var fileClient = directoryClient.GetFileClient(file.FileName);
            using (var stream = file.OpenReadStream())
            {
                await fileClient.CreateAsync(stream.Length);
                await fileClient.UploadRangeAsync(new HttpRange(0, stream.Length), stream);
            }

            log.LogInformation($"File '{file.FileName}' uploaded to '{directoryName}' in file share '{_fileShareName}'.");

            return new OkObjectResult($"File '{file.FileName}' uploaded successfully to '{directoryName}'.");
        }
    }
}
