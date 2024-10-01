using Microsoft.AspNetCore.Mvc;
using POECLDPart1.Models;
using POECLDPart1.Services;

namespace POECLDPart1.Controllers
{
    public class SoccerBootsController : Controller
    {
        private readonly BlobService _blobService;
        private readonly TableStorageService _tableStorageService;

        public SoccerBootsController(BlobService blobService, TableStorageService tableStorageService)
        {
            _blobService = blobService;
            _tableStorageService = tableStorageService;
        }

        [HttpGet]
        public IActionResult AddSoccerBoot()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddSoccerBoot(SoccerBoot soccerBoot, IFormFile file)
        {
            if (file != null)
            {
                using var stream = file.OpenReadStream();
                var imageUrl = await _blobService.UploadAsync(stream, file.FileName);
                soccerBoot.ImageUrl = imageUrl;
            }

            soccerBoot.PartitionKey = "SoccerBootPartition";
            soccerBoot.RowKey = Guid.NewGuid().ToString();

            await _tableStorageService.AddSoccerBootsAsync(soccerBoot);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSoccerBoot(string partitionKey, string rowKey, SoccerBoot soccerBoot)
        {

            if (soccerBoot != null && !string.IsNullOrEmpty(soccerBoot.ImageUrl))
            {
                // Delete the associated blob image
                await _blobService.DeleteBlobAsync(soccerBoot.ImageUrl);
            }
            //Delete Table entity
            await _tableStorageService.DeleteSoccerBootAsync(partitionKey, rowKey);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index()
        {
            var soccerBoots = await _tableStorageService.GetAllSoccerBootsAsync();
            return View(soccerBoots);
        }
    }

}


 