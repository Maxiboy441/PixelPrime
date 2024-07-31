using Microsoft.AspNetCore.Mvc;
using Project.Services;

namespace Project.Controllers
{
    public class ApiController : ControllerBase
    {
        private readonly CacheService _cache;

        public ApiController(CacheService cache)
        {
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Search(string s)
        {
            try
            {
                var response = await _cache.GetJsonAsyncByName(s);

                return Content(response, "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Search action: {ex.Message}");

                return StatusCode(500, new { error = "An error occurred while processing your request." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchById(string id)
        {
            try
            {
                var response = await _cache.GetJsonAsyncById(id);

                return Content(response, "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Search action: {ex.Message}");

                return StatusCode(500, new { error = "An error occurred while processing your request." });
            }
        }
    }
}
