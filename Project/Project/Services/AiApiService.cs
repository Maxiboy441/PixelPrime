using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Project.Services
{
    public class AiApiService
    {
        private readonly HttpClient _client;
        private readonly string _apiUrl;
        private readonly ILogger<AiApiService> _logger;
        private readonly string _aiToken;

        public AiApiService(HttpClient client, IConfiguration config, ILogger<AiApiService> logger)
        {
            _client = client;
            _apiUrl = config.GetValue<string>("Api:AiURL") ?? throw new ArgumentNullException(nameof(_apiUrl));
            _logger = logger;
            _aiToken = config.GetValue<string>("Api:AiToken") ?? throw new ArgumentNullException(nameof(_aiToken));
        }

        public async Task<List<string>> GenerateResponse(string prompt)
        {
            _logger.LogInformation("Generating response for prompt: {Prompt}", prompt);

            var requestBody = new
            {
                model = "mistral:latest",
                prompt = prompt,
                stream = false
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending POST request to {ApiUrl}", _apiUrl);
            
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _aiToken);

            var response = await _client.PostAsync(_apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received response: {ResponseContent}", responseContent);

                try
                {
                    List<string> movieList = ExtractMovieNames(responseContent);
                    _logger.LogInformation("MovieList: {movieList}", movieList);
                    return movieList;
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Couldnt make : {responseContent} a Movie list", responseContent);
                    throw new Exception("Couldnt make a Movie list");
                }
            }
            else
            {
                _logger.LogError("API request failed with status code: {StatusCode}", response.StatusCode);
                throw new Exception($"API request failed with status code: {response.StatusCode}");
            }
        }
        
        public static List<string> ExtractMovieNames(string jsonResponse)
        {
            try
            {
                // Parse the JSON string into a JsonDocument
                using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                {
                    // Access the "response" field which contains the list of movie names
                    var responseElement = doc.RootElement.GetProperty("response").GetString();

                    // Clean the response string by removing unwanted characters
                    responseElement = responseElement.Replace("[", "")
                        .Replace("]", "")
                        .Replace("\"", "")
                        .Trim();

                    // Split the cleaned response into individual movie names, ensuring no empty or whitespace entries
                    var movieNames = responseElement.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    // Create a list of trimmed movie names and avoid leading/trailing spaces
                    List<string> movieList = new List<string>();
                    foreach (var movie in movieNames)
                    {
                        string trimmedMovie = movie.Trim();
                        if (!string.IsNullOrWhiteSpace(trimmedMovie)) // Ensure no empty strings
                        {
                            movieList.Add(trimmedMovie);
                        }
                    }

                    return movieList;
                }
            }
            catch
            {
                throw new Exception("Error while extracting movie names");
            }
        }
    }
}