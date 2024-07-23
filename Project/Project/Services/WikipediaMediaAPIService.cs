using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class WikipediaMediaAPIService
{
    private readonly HttpClient _httpClient;
    private readonly SemaphoreSlim _rateLimiter;
    private const string BaseUrl = "https://en.wikipedia.org/api/rest_v1/page/media-list/";
    private const string UserAgent = "WikipediaMediaAPIService/1.0 (https://pixelprime.maxih.de)";
    private const int MaxRequestsPerSecond = 200;

    public WikipediaMediaAPIService()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
        _rateLimiter = new SemaphoreSlim(MaxRequestsPerSecond, MaxRequestsPerSecond);
    }

    public async Task<string> GetFirstImageUrlAsync(string actorName)
    {
        try
        {
            await _rateLimiter.WaitAsync();
            
            try
            {
                string encodedName = Uri.EscapeDataString(actorName);
                string apiUrl = $"{BaseUrl}{encodedName}";

                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                string jsonContent = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(jsonContent);

                JsonElement root = doc.RootElement;
                JsonElement items = root.GetProperty("items");

                if (items.GetArrayLength() > 0)
                {
                    JsonElement firstImage = items[0];
                    JsonElement srcset = firstImage.GetProperty("srcset");

                    if (srcset.GetArrayLength() > 0)
                    {
                        string imageUrl = srcset[0].GetProperty("src").GetString();
                        return "http:" + imageUrl;
                    }
                }

                return "No image found";
            }
            finally
            {
                // Schedule the release of the semaphore after 1 second
                _ = Task.Delay(1000).ContinueWith(_ => _rateLimiter.Release());
            }
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}