using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace POECLDPart1.Models
{
    public class SoccerBoot : ITableEntity
    {
        [Key]
        public int SoccerBoot_Id { get; set; }  // Ensure this property exists and is populated
        public string? SoccerBoot_Name { get; set; }  // Ensure this property exists and is populated
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int Price { get; set; }



        // Implementing ITableEntity properties
        public string PartitionKey { get; set; } = "SoccerBoots";  // PartitionKey, typically the category or type of entity
        public string RowKey { get; set; }                   // Unique identifier within the partition, e.g., Book_Id as a string
        public DateTimeOffset? Timestamp { get; set; }       // Timestamp for concurrency
        public ETag ETag { get; set; }                       // ETag for concurrency checks
    }
}
