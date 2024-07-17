using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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

                try
                {
                    var responseObject = JsonSerializer.Deserialize<JsonElement>(responseContent);

                    if (responseObject.TryGetProperty("response", out var responseProperty))
                    {
                        var responseString = responseProperty.GetString();
                        var jsonContent = ConvertToValidJson(ExtractJsonFromMarkdown(responseString));

                        if (!string.IsNullOrEmpty(jsonContent))
                        {
                            try
                            {
                                var movieList = JsonSerializer.Deserialize<List<string>>(jsonContent);
                                return movieList;
                            }
                            catch (JsonException ex)
                            {
                                _logger.LogError(ex, "Failed to deserialize JSON content: {JsonContent}", jsonContent);
                                throw new Exception("Failed to deserialize JSON content", ex);
                            }
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
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Failed to parse API response as JSON: {ResponseContent}", responseContent);
                    throw new Exception("Failed to parse API response as JSON", ex);
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
            var match = Regex.Match(markdown, @"```json\s*([\s\S]*?)\s*```");
            return match.Success ? match.Groups[1].Value.Trim() : string.Empty;
        }
        
        public static string ConvertToValidJson(string input)
        {
            string cleanedInput = Regex.Replace(input, @"\s+", " ").Trim();

            cleanedInput = Regex.Replace(cleanedInput, @"^```json?\s*|\s*```$", "");

            try
            {
                var jsonArray = JArray.Parse(cleanedInput);

                return JsonConvert.SerializeObject(jsonArray, Formatting.Indented);
            }
            catch (JsonException)
            {
                string[] items = Regex.Split(cleanedInput, @",\s*(?=(?:[^""]*""[^""]*"")*[^""]*$)");

                var validItems = new JArray();
                foreach (var item in items)
                {
                    string trimmedItem = item.Trim().Trim('[', ']', '"');
                    if (!string.IsNullOrWhiteSpace(trimmedItem))
                    {
                        validItems.Add(trimmedItem);
                    }
                }
                
                return JsonConvert.SerializeObject(validItems, Formatting.Indented);
            }
        }
    }
}