using Azure.Data.Tables;
using Azure;
using POECLDPart1.Models;

namespace POECLDPart1.Services
{
    public class TableStorageService
    {
        private readonly TableClient _tableClient;
        private readonly TableClient _customerTableClient;
        private readonly TableClient _orderTableClient;

        public TableStorageService(string connectionString)
            {
            _tableClient = new TableClient(connectionString, "SoccerBoots");
            _customerTableClient = new TableClient(connectionString, "Customer");
            _orderTableClient = new TableClient(connectionString, "Order");
            }

            // Product-related methods
            public async Task<List<SoccerBoot>> GetAllSoccerBootsAsync()
            {
                var soccerBoots = new List<SoccerBoot>();

                await foreach (var soccerBoot in _tableClient.QueryAsync<SoccerBoot>())
                {
                soccerBoots.Add(soccerBoot);
                }

                return soccerBoots;
            }

        public async Task AddSoccerBootsAsync(SoccerBoot soccerBoot)
        {
            if (string.IsNullOrEmpty(soccerBoot.PartitionKey) || string.IsNullOrEmpty(soccerBoot.RowKey))
            {
                throw new ArgumentException("PartitionKey and RowKey must be set.");
            }

            try
            {
                await _tableClient.AddEntityAsync(soccerBoot);
            }
            catch (RequestFailedException ex)
            {
                throw new InvalidOperationException("Error adding entity to Table Storage", ex);
            }
        }

        public async Task DeleteSoccerBootAsync(string partitionKey, string rowKey)
            {
                await _tableClient.DeleteEntityAsync(partitionKey, rowKey);
            }

            public async Task<SoccerBoot?> GetSoccerBootAsync(string partitionKey, string rowKey)
            {
                try
                {
                    var response = await _tableClient.GetEntityAsync<SoccerBoot>(partitionKey, rowKey);
                    return response.Value;
                }
                catch (RequestFailedException ex) when (ex.Status == 404)
                {
                    return null;
                }
            }
            public async Task<SoccerBoot?> GetSoccerBootsAsync(string SoccerBoot_Id)
            {
                var soccerBoots = new List<SoccerBoot>();
                await foreach (var soccerBoot in _tableClient.QueryAsync<SoccerBoot>(filter: $"ProductId eq '{SoccerBoot_Id}'"))
                {
                soccerBoots.Add(soccerBoot);
                }
                return soccerBoots.FirstOrDefault();
            }

            // Customer-related methods
            public async Task<List<Customer>> GetAllCustomersAsync()
            {
                var customers = new List<Customer>();

                await foreach (var customer in _customerTableClient.QueryAsync<Customer>())
                {
                    customers.Add(customer);
                }

                return customers;
            }

            public async Task AddCustomerAsync(Customer customer)
            {
                if (string.IsNullOrEmpty(customer.PartitionKey) || string.IsNullOrEmpty(customer.RowKey))
                {
                    throw new ArgumentException("PartitionKey and RowKey must be set.");
                }

                try
                {
                    await _customerTableClient.AddEntityAsync(customer);
                }
                catch (RequestFailedException ex)
                {
                    throw new InvalidOperationException("Error adding entity to Table Storage", ex);
                }
            }

            public async Task DeleteCustomerAsync(string partitionKey, string rowKey)
            {
                await _customerTableClient.DeleteEntityAsync(partitionKey, rowKey);
            }

            public async Task<Customer?> GetCustomerAsync(string partitionKey, string rowKey)
            {
                try
                {
                    var response = await _customerTableClient.GetEntityAsync<Customer>(partitionKey, rowKey);
                    return response.Value;
                }
                catch (RequestFailedException ex) when (ex.Status == 404)
                {
                    return null;
                }
            }

            // Order-related methods
            public async Task<List<Order>> GetAllOrdersAsync()
            {
                var orders = new List<Order>();

                await foreach (var order in _orderTableClient.QueryAsync<Order>())
                {
                    orders.Add(order);
                }

                return orders;
            }

            public async Task AddOrderAsync(Order order)
            {
                if (string.IsNullOrEmpty(order.PartitionKey) || string.IsNullOrEmpty(order.RowKey))
                {
                    throw new ArgumentException("PartitionKey and RowKey must be set.");
                }

                try
                {
                    await _orderTableClient.AddEntityAsync(order);
                }
                catch (RequestFailedException ex)
                {
                    throw new InvalidOperationException("Error adding entity to Table Storage", ex);
                }
            }

            public async Task DeleteOrderAsync(string partitionKey, string rowKey)
            {
                await _orderTableClient.DeleteEntityAsync(partitionKey, rowKey);
            }



            public async Task<Order?> GetOrderAsync(string partitionKey, string rowKey)
            {
                try
                {
                    var response = await _orderTableClient.GetEntityAsync<Order>(partitionKey, rowKey);
                    return response.Value;
                }
                catch (RequestFailedException ex) when (ex.Status == 404)
                {
                    return null;
                }
            }
        }
    }

