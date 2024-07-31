using Microsoft.Extensions.Caching.Memory;
using Project.Models;

namespace Project.Services
{
	public class CacheService
	{
        private readonly IMemoryCache _memoryCache;
        private readonly MovieApiService _movieApiService;
        public CacheService(IMemoryCache memoryCache, MovieApiService movieApiService)
		{
            _memoryCache = memoryCache;
            _movieApiService = movieApiService;
        }

        public async Task<Movie> GetContentAsync(string id)
        {
            if (!_memoryCache.TryGetValue($"movie_{id}", out Movie? movie))
            {
                movie = await _movieApiService.GetMovieById(id);

                if (movie == null)
                {
                    return null;
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6),
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                };

                _memoryCache.Set($"movie_{id}", movie, cacheEntryOptions);
            }
            else
            {
                Console.WriteLine($"Content fetched from cache");
            }
            return movie;
        }
    }
}
