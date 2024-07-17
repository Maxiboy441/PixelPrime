using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Project.Services;

public class AiApiService
{
    private readonly string _apiUrl;
    private readonly HttpClient _client;
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
            prompt,
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

                    _logger.LogError("No valid JSON found in the response: {ResponseString}", responseString);
                    throw new Exception("No valid JSON found in the response");
                }

                _logger.LogError("Response property not found in API response: {ResponseContent}", responseContent);
                throw new Exception("Response property not found in API response");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse API response as JSON: {ResponseContent}", responseContent);
                throw new Exception("Failed to parse API response as JSON", ex);
            }
        }

        _logger.LogError("API request failed with status code: {StatusCode}", response.StatusCode);
        throw new Exception($"API request failed with status code: {response.StatusCode}");
    }

    private string ExtractJsonFromMarkdown(string markdown)
    {
        var match = Regex.Match(markdown, @"```json\s*([\s\S]*?)\s*```");
        return match.Success ? match.Groups[1].Value.Trim() : string.Empty;
    }

    public static string ConvertToValidJson(string input)
    {
        var cleanedInput = Regex.Replace(input, @"\s+", " ").Trim();
        cleanedInput = Regex.Replace(cleanedInput, @"^```json?\s*|\s*```$", "");

        try
        {
            var jsonToken = JToken.Parse(cleanedInput);
            return JsonConvert.SerializeObject(jsonToken, Formatting.Indented);
        }
        catch (JsonException)
        {
            var result = new JObject();
            var mainArray = new JArray();
            var currentObject = new JObject();
            var insideObject = false;

            var items = Regex.Split(cleanedInput, @",\s*(?=(?:[^""]*""[^""]*"")*[^""]*$)");
            foreach (var item in items)
            {
                var trimmedItem = item.Trim().Trim('[', ']', '"');
                if (string.IsNullOrWhiteSpace(trimmedItem)) continue;

                if (trimmedItem.Contains(":"))
                {
                    var parts = trimmedItem.Split(':', 2);
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim().Trim('"');
                        var value = parts[1].Trim().Trim('"');

                        if (key.EndsWith("{"))
                        {
                            insideObject = true;
                            currentObject = new JObject();
                            result[key.TrimEnd('{')] = currentObject;
                        }
                        else if (insideObject)
                        {
                            currentObject[key] = TryParseValue(value);
                        }
                        else
                        {
                            result[key] = TryParseValue(value);
                        }
                    }
                }
                else if (trimmedItem == "}")
                {
                    insideObject = false;
                }
                else
                {
                    mainArray.Add(trimmedItem);
                }
            }

            if (mainArray.Count > 0) result["items"] = mainArray;

            return JsonConvert.SerializeObject(result, Formatting.Indented);
        }
    }

    private static JToken TryParseValue(string value)
    {
        if (double.TryParse(value, out var numericValue))
            return numericValue;
        if (bool.TryParse(value, out var boolValue))
            return boolValue;
        return value;
    }
}