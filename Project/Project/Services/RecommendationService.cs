using System.Text;
using Microsoft.EntityFrameworkCore;
using Polly;
using Project.Data;
using Project.Models;

namespace Project.Services
{
    public class RecommendationService
    {
        private readonly DataContext _context;
        private readonly AiApiService _aiApiService;
        private readonly MovieApiService _movieApiService;
        private readonly ILogger<RecommendationService> _logger;

        public RecommendationService(DataContext context, AiApiService aiApiService, MovieApiService movieApiService, ILogger<RecommendationService> logger)
        {
            _context = context;
            _aiApiService = aiApiService;
            _movieApiService = movieApiService;
            _logger = logger;
        }

        public async Task GetRecommendations(int userId)
{
    var retryPolicy = Policy
        .Handle<Exception>(ex => 
            ex.Message.Contains("Error while extracting movie names") ||
            ex.Message.Contains("API request failed with status code") ||
            ex.Message.Contains("Couldn't make a Movie list")
            ) 
        .WaitAndRetryAsync(
            3, 
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            (exception, timeSpan, retryCount, context) =>
            {
                _logger.LogWarning($"Retry {retryCount} for GetRecommendations due to: {exception.Message}. Waiting {timeSpan.TotalSeconds} seconds before next retry.");
            }
        );

    await retryPolicy.ExecuteAsync(async () =>
    {
        List<Rating> liked = await GetLikedMovies(userId);
        List<Favorite> favorites = await GetFavorites(userId);

        string prompt1 = GenerateRatingsString(liked);
        string prompt2 = GenerateFavoritesString(favorites);

        string finalPrompt = $"Recommend me new movies.\n{prompt2}\n{prompt1}\nGive me a json of the movie names, nothing more!";
        
        List<string> response = await _aiApiService.GenerateResponse(finalPrompt);
        _logger.LogInformation("AI API Response: {response}", string.Join(", ", response));

        DateTime fourDaysAgo = DateTime.Now.AddDays(-8);
        await _context.Recommendations
            .Where(r => r.User_id == userId && r.Created_at < fourDaysAgo)
            .ExecuteDeleteAsync();

        foreach (var title in response)
        {
            _logger.LogInformation("Processing movie: {title}", title);

            Movie movie = await _movieApiService.GetMovieByName(title);

            if (movie.Title != "error")
            {
                _logger.LogInformation("Movie found: {movieTitle}, {movieId}", movie.Title, movie.Id);

                Recommendation recommendation = new Recommendation
                {
                    Movie_id = movie.Id,
                    User_id = userId,
                    Movie_title = movie.Title,
                    Movie_poster = movie.Poster,
                    Created_at = DateTime.Now,
                    Updated_at = DateTime.Now
                };
                
                /*
                bool isInFavorites = await _context.Favorites
                    .AnyAsync(f => f.User_id == userId && f.Movie_id == movie.Id);

                bool isInRatings = await _context.Ratings
                    .AnyAsync(r => r.User_id == userId && r.Movie_id == movie.Id);

                bool isInRecommendations = await _context.Recommendations
                    .AnyAsync(r => r.User_id == userId && r.Movie_id == movie.Id);
                */

                // TODO: check if recommendations is duplicate or already rated or favorite of the user
                if (true)
                {
                    _logger.LogInformation("Adding recommendation: {movieTitle}", movie.Title);

                    _context.Recommendations.Add(recommendation);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Recommendation saved: {movieTitle}", movie.Title);
                }
                else
                {
                    _logger.LogInformation("Movie already exists in favorites, ratings, or recommendations: {movieTitle}", movie.Title);
                }
            }
            else
            {
                _logger.LogWarning("Movie not found or error returned: {title}", title);
            }
        }
    });
}


        public async Task<List<Rating>> GetLikedMovies(int userId)
        {
            return await _context.Ratings
                .Where(r => r.User_id == userId && r.Rating_value > 7.5)
                .OrderByDescending(r => r.Created_at)
                .Take(10)
                .ToListAsync();
        }

        public async Task<List<Favorite>> GetFavorites(int userId)
        {
            return await _context.Favorites
                .Where(r => r.User_id == userId)
                .ToListAsync();
        }

        private string GenerateRatingsString(List<Rating> ratings)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("I rated following movies as such:");

            foreach (var rating in ratings)
            {
                sb.AppendLine($"{rating.Movie_title}, {rating.Rating_value}");
            }

            return sb.ToString();
        }

        private string GenerateFavoritesString(List<Favorite> ratings)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("My favorite movies of all times are");

            foreach (var rating in ratings)
            {
                sb.AppendLine($"{rating.Movie_title}");
            }

            return sb.ToString();
        }

        public async Task<DateTime?> GetNewestRecommendationDate(int userId)
        {
            return await _context.Recommendations
                .Where(r => r.User_id == userId)
                .OrderByDescending(r => r.Created_at)
                .Select(r => r.Created_at)
                .FirstOrDefaultAsync();
        }
    }
}