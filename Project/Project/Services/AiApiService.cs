using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Project.Services;

public class AiApiService
{
    private readonly HttpClient _client;
    private readonly string? _apiUrl;

    public AiApiService(HttpClient client, IConfiguration config)
    {
        _client = client;
        _apiUrl = config.GetValue<string>("Api:AiURL");
    }
    
    public async Task<List<string>> GenerateResponse(string prompt)
    {
        var requestBody = new
        {
            model = "movie-critic:latest",
            prompt = prompt,
            stream = false
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(_apiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
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
                    throw new Exception("No valid JSON found in the response");
                }
            }
            else
            {
                throw new Exception("Response property not found in API response");
            }
        }
        else
        {
            throw new Exception($"API request failed with status code: {response.StatusCode}");
        }
    }

    private string ExtractJsonFromMarkdown(string markdown)
    {
        var match = System.Text.RegularExpressions.Regex.Match(markdown, @"```json\s*([\s\S]*?)\s*```");
        return match.Success ? match.Groups[1].Value.Trim() : string.Empty;
    }
}