using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using POECLDPart1.Models;
using POECLDPart1.Services;

namespace POECLDPart1.Controllers
{
    public class OrdersController : Controller
    {
       

        private readonly TableStorageService _tableService;
        private readonly QueueService _queueService;

        public OrdersController(TableStorageService tableService, QueueService queueService)
        {
            _tableService = tableService;
            _queueService = queueService;
        }

        // Action to display all ORDERS

        [HttpPost]
        public async Task<IActionResult> CreateOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return View(order);
            }

            // Set the PartitionKey and RowKey properties
            order.PartitionKey = order.SoccerBoot_ID.ToString(); // Replace with the actual partition key --> ADDED THIS
            order.RowKey = Guid.NewGuid().ToString(); // Generate a unique row key

            await _tableService.AddOrderAsync(order);

            return RedirectToAction("Index", "Orders"); //added this method

        }


        public async Task<IActionResult> Index()
        {
            var orders = await _tableService.GetAllOrdersAsync();
            return View(orders);
        }


        public async Task<IActionResult> CreateOrder()
        {
            var customers = await _tableService.GetAllCustomersAsync();
            var soccerBoots = await _tableService.GetAllSoccerBootsAsync();

            if (customers == null || customers.Count == 0 || soccerBoots == null || soccerBoots.Count == 0)
            {
                // Set the error message to display in the view
                ModelState.AddModelError("", "No customers or books available, please ensure they are added first.");
                return View(); // Return the view with the error message
            }

            ViewData["Customers"] = customers;
            ViewData["SoccerBoots"] = soccerBoots;

            return View();
        }

        private async Task<int> GenerateUniqueID()
        {
            // get all id's 
            var orders = await _tableService.GetAllOrdersAsync();

            //check for existig customer id
            if (orders.Any())
            {
                // get max id and + 1
                int maxID = orders.Max(o => o.Order_Id);
                return maxID + 1;
            }

            // if no existing customer, start at 1
            return 1;
        }


        // Action to handle the form submission and register the sighting
        [HttpPost]
        public async Task<IActionResult> AddOrder(Order order)
        {
            if (ModelState.IsValid)
            {
                //TableService
                order.PartitionKey = "OrdersPartition";
                order.RowKey = Guid.NewGuid().ToString();
                order.Order_Id = await GenerateUniqueID();
                await _tableService.AddOrderAsync(order);

                //MessageQueue
                string message = $"New order placed by customer ID {order.Customer_ID} of product ID {order.SoccerBoot_ID} for amount of R{order.TotalPrice}";
                await _queueService.SendMessageAsync(message);

                return RedirectToAction("Index");
            }
            else
            {
                // Log model state errors
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Key: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
            }

            var customers = await _tableService.GetAllCustomersAsync();
            var soccerBoots = await _tableService.GetAllSoccerBootsAsync();
            ViewData["Customers"] = customers;
            ViewData["SoccerBoots"] = soccerBoots;

            return View(order);
        }



    }
}




    