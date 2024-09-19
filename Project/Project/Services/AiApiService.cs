using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

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
                    _logger.LogError(ex, "Couldn't make : {responseContent} a Movie list", responseContent);
                    throw new Exception("Couldn't make a Movie list");
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
                using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                {
                    var responseElement = doc.RootElement.GetProperty("response").GetString();
                    var movieArray = JsonDocument.Parse(responseElement).RootElement;
                    
                    return movieArray.EnumerateArray()
                        .Select(movie => movie.GetString())
                        .Where(title => !string.IsNullOrWhiteSpace(title))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while extracting movie names: " + ex.Message);
            }
        }
    }
}