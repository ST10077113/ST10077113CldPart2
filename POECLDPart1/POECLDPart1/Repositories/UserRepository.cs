using Azure.Data.Tables;
using Azure;
using POECLDPart1.Models;

namespace POECLDPart1.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TableClient _tableClient;

        public UserRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AzureStorage");
            var tableName = configuration.GetSection("AzureTableSettings")["Users"];

            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName), "Table name cannot be null or empty.");
            }

            _tableClient = new TableClient(connectionString, tableName);
            _tableClient.CreateIfNotExists();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                // assuming rowkey is email
                var response = await _tableClient.GetEntityAsync<User>("User", email);
                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            try
            {
                await _tableClient.AddEntityAsync(user);
                return true;
            }
            catch (RequestFailedException)
            {
                // handle exception
                return false;
            }
        }
    }
}
