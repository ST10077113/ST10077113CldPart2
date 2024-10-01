using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace POECLDPart1.Models
{
    public class Order : ITableEntity
    {
        [Key]
        public int Order_Id { get; set; }

        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        //Introduce validation sample
        [Required(ErrorMessage = "Please select a customer.")]
        public int Customer_ID { get; set; } // FK to the customer who made the order

        [Required(ErrorMessage = "Please select a soccer boot.")]
        public int SoccerBoot_ID { get; set; } // FK to the soccer boot being ordered

        public double TotalPrice { get; set; }
    }
}
