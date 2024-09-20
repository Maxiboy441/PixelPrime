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

        public AiApiService(HttpClient client, IConfiguration config, ILogger<AiApiService> logger)
        {
            _client = client;
            _apiUrl = config.GetValue<string>("Api:AiURL") ?? throw new ArgumentNullException(nameof(_apiUrl));
            _logger = logger;

            var aiToken = config.GetValue<string>("Api:AiToken");

            if (string.IsNullOrEmpty(aiToken))
            {
                throw new ArgumentNullException(nameof(aiToken));
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", aiToken);

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
                    _logger.LogError(ex, "Couldn't convert response to movie list: {ResponseContent}", responseContent);
                    throw new InvalidOperationException("Couldn't convert response to a movie list", ex);
                }
            }
            else
            {
                _logger.LogError("API request failed with status code: {StatusCode}", response.StatusCode);
                throw new HttpRequestException($"API request failed with status code: {response.StatusCode}");
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
                throw new InvalidOperationException("Error while extracting movie names", ex);
            }
        }
    }
}
