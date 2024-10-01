using Newtonsoft.Json;
using System.Text;

namespace POECLDPart1.Services
{
    public class FunctionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _sendMessageUrl;
        private readonly string _uploadBlobUrl;
        private readonly string _uploadFileUrl;

        public FunctionService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _sendMessageUrl = configuration["AzureFunctions:SendMessageToQueue"];
            _uploadBlobUrl = configuration["AzureFunctions:UploadToBlob"];
            _uploadFileUrl = configuration["AzureFunctions:UploadFileToShare"];
        }

       

        public async Task<bool> SendMessageToQueueAsync(string message)
        {
            var content = new StringContent(JsonConvert.SerializeObject(new { message }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_sendMessageUrl, content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UploadToBlobAsync(Stream fileStream, string fileName)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(fileStream), "file", fileName);

            var response = await _httpClient.PostAsync(_uploadBlobUrl, content);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> UploadFileToShareAsync(Stream fileStream, string fileName, string directoryName)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(fileStream), "file", fileName);
            content.Add(new StringContent(directoryName), "directoryName");

            var response = await _httpClient.PostAsync(_uploadFileUrl, content);
            return response.IsSuccessStatusCode;
        }
    }
}
