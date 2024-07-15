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
        }

        public async Task<List<string>> GenerateResponse(string prompt)
        {
            _logger.LogInformation("Generating response for prompt: {Prompt}", prompt);

            var requestBody = new
            {
                model = "movie-critic:latest",
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

                var responseObject = JsonSerializer.Deserialize<JsonElement>(responseContent);

                if (responseObject.TryGetProperty("response", out var responseProperty))
                {
                    var responseString = responseProperty.GetString();
                    var jsonContent = ExtractJsonFromMarkdown(responseString);

                    if (!string.IsNullOrEmpty(jsonContent))
                    {
                        var movieList = JsonSerializer.Deserialize<List<string>>(jsonContent);
                        return movieList;
                    }
                    else
                    {
                        _logger.LogError("No valid JSON found in the response: {ResponseString}", responseString);
                        throw new Exception("No valid JSON found in the response");
                    }
                }
                else
                {
                    _logger.LogError("Response property not found in API response: {ResponseContent}", responseContent);
                    throw new Exception("Response property not found in API response");
                }
            }
            else
            {
                _logger.LogError("API request failed with status code: {StatusCode}", response.StatusCode);
                throw new Exception($"API request failed with status code: {response.StatusCode}");
            }
        }

        private string ExtractJsonFromMarkdown(string markdown)
        {
            var match = System.Text.RegularExpressions.Regex.Match(markdown, @"```json\s*([\s\S]*?)\s*```");
            return match.Success ? match.Groups[1].Value.Trim() : string.Empty;
        }
    }
}
