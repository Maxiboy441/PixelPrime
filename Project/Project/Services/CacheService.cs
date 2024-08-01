using System.Web;
using Microsoft.Extensions.Caching.Memory;
using Project.Models;

namespace Project.Services
{
	public class CacheService
	{
        private readonly IMemoryCache _memoryCache;
        private readonly MovieApiService _movieApiService;
        private readonly HttpClient _httpClient;
        private readonly string? _apiKey;

        public CacheService(IMemoryCache memoryCache, MovieApiService movieApiService, HttpClient httpClient, IConfiguration configuration)
		{
            _memoryCache = memoryCache;
            _movieApiService = movieApiService;
            _httpClient = httpClient;
            _apiKey = configuration["Api:MovieApi"];
        }

        public async Task<(Movie? movie, string jsonResponse)> FetchACacheContentAsync(string id)
        {
            var encodedSearchTerm = HttpUtility.UrlEncode(id);
            var apiUrl = $"https://www.omdbapi.com/?apikey={_apiKey}&i={encodedSearchTerm}";

            if (!_memoryCache.TryGetValue($"content_{id}", out (Movie movie, string jsonResponse) cachedContent))
            {
                var jsonResponse = await _httpClient.GetStringAsync(apiUrl);

                var movie = await _movieApiService.GetMovieById(id);
                if (movie == null)
                {
                    return (null, jsonResponse);
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6),
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                };

                _memoryCache.Set($"content_{id}", (movie, jsonResponse), cacheEntryOptions);

                return (movie, jsonResponse);
            }
            else
            {
                Console.WriteLine($"Content fetched from cache");
                return cachedContent;
            }
        }

        public async Task<Movie?> GetContentAsync(string id)
        {
            var (movie, _) = await FetchACacheContentAsync(id);
            return movie;
        }

        public async Task<string> GetJsonAsyncById(string id)
        {
            var (_, jsonResponse) = await FetchACacheContentAsync(id);
            return jsonResponse;
        }

        //public async Task<string> GetJsonAsyncById(string id)
        //{
        //    var encodedSearchTerm = HttpUtility.UrlEncode(id);
        //    var apiUrl = $"https://www.omdbapi.com/?apikey={_apiKey}&s={encodedSearchTerm}";

        //    if (!_memoryCache.TryGetValue($"content_{id}", out string? response))
        //    {
        //        response = await _httpClient.GetStringAsync(apiUrl);

        //        var cacheEntryOptions = new MemoryCacheEntryOptions
        //        {
        //            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6),
        //            SlidingExpiration = TimeSpan.FromMinutes(30)
        //        };

        //        _memoryCache.Set($"content_{id}", response, cacheEntryOptions);
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Data fetched by id from cache");
        //    }

        //    return response;
        //}

        public async Task<string> GetJsonAsyncByName(string term)
        {
            var encodedSearchTerm = HttpUtility.UrlEncode(term);
            var apiUrl = $"https://www.omdbapi.com/?apikey={_apiKey}&s={encodedSearchTerm}";
            var cacheKey = $"search_{term}";

            if (!_memoryCache.TryGetValue($"search_{cacheKey}", out string? response))
            {
                response = await _httpClient.GetStringAsync(apiUrl);

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6),
                    SlidingExpiration = TimeSpan.FromMinutes(1)
                };

                _memoryCache.Set($"search_{cacheKey}", response, cacheEntryOptions);
            }
            else
            {
                Console.WriteLine($"Data fetched by name from cache");
            }

            return response;
        }
    }
}
