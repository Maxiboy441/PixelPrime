using System.Text;

using System.Text.Json;

namespace Project.Services;

public class AiApiService
{
    private static readonly HttpClient client = new HttpClient();
    
    private static readonly IConfigurationRoot Myconfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    private static readonly string? ApiURL = Myconfig.GetValue<string>("Api:AiURL");
    
    public static async Task<string> GenerateResponse(string prompt)
    {
        var requestBody = new
        {
            model = "movie-critic:latest",
            prompt = prompt,
            stream = false
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(ApiURL, content);

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
                    return JsonSerializer.Serialize(movieList, new JsonSerializerOptions { WriteIndented = true });
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

    private static string ExtractJsonFromMarkdown(string markdown)
    {
        var match = System.Text.RegularExpressions.Regex.Match(markdown, @"```json\s*([\s\S]*?)\s*```");
        return match.Success ? match.Groups[1].Value.Trim() : string.Empty;
    }
}