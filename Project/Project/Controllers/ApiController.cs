using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Project.Controllers
{
    public class ApiController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string? _apiKey;

        public ApiController(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Api:MovieApi"];
        }

        [HttpGet]
        public async Task<IActionResult> Search(string s)
        {
            try
            {
                var encodedSearchTerm = HttpUtility.UrlEncode(s);
                var apiUrl = $"https://www.omdbapi.com/?apikey={_apiKey}&s={encodedSearchTerm}";

                var response = await _httpClient.GetStringAsync(apiUrl);

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
                var encodedSearchTerm = HttpUtility.UrlEncode(id);
                Console.WriteLine($"id{encodedSearchTerm}");
                var apiUrl = $"https://www.omdbapi.com/?apikey={_apiKey}&i={id}";

                var response = await _httpClient.GetStringAsync(apiUrl);

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
