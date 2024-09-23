using System.Text;
using Microsoft.EntityFrameworkCore;
using Polly;
using Project.Data;
using Project.Models;

namespace Project.Services;

public class RecommendationService
{
    private readonly AiApiService _aiApiService;
    private readonly DataContext _context;
    private readonly ILogger<RecommendationService> _logger;
    private readonly MovieApiService _movieApiService;

    public RecommendationService(DataContext context, AiApiService aiApiService, MovieApiService movieApiService,
        ILogger<RecommendationService> logger)
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
                    _logger.LogWarning(
                        $"Retry {retryCount} for GetRecommendations due to: {exception.Message}. Waiting {timeSpan.TotalSeconds} seconds before next retry.");
                }
            );

        await retryPolicy.ExecuteAsync(async () =>
        {
            var liked = await GetLikedMovies(userId);
            var favorites = await GetFavorites(userId);

            var prompt1 = GenerateRatingsString(liked);
            var prompt2 = GenerateFavoritesString(favorites);

            var finalPrompt =
                $"Based on the following information about my movie preferences, recommend me NEW movies that I haven't seen before. Do not include any movies I've already mentioned.\n\n{prompt2}\n{prompt1}\n\nGive me a json array of ONLY the new recommended movie names, nothing more. Format: [\"Movie 1\", \"Movie 2\", \"Movie 3\"]";

            var response = await _aiApiService.GenerateResponse(finalPrompt);
            _logger.LogInformation("AI API Response: {response}", string.Join(", ", response));

            var fourDaysAgo = DateTime.Now.AddDays(-8);
            await _context.Recommendations
                .Where(r => r.User_id == userId && r.Created_at < fourDaysAgo)
                .ExecuteDeleteAsync();

            _logger.LogInformation("Starting to process {count} movies", response.Count);

            try
            {
                foreach (var title in response)
                {
                    _logger.LogInformation("Starting to process movie: {title}", title);

                    var movie = await _movieApiService.GetMovieByName(title);

                    if (movie != null && movie.Title != "error")
                    {
                        _logger.LogInformation("Movie found: {movieTitle}, {movieId}", movie.Title, movie.Id);

                        var recommendation = new Recommendation
                        {
                            Movie_id = movie.Id,
                            User_id = userId,
                            Movie_title = movie.Title,
                            Movie_poster = movie.Poster,
                            Created_at = DateTime.Now,
                            Updated_at = DateTime.Now
                        };

                        var isInFavorites = await _context.Favorites
                            .AnyAsync(f => f.User_id == userId && f.Movie_id == movie.Id);

                        var isInRatings = await _context.Ratings
                            .AnyAsync(r => r.User_id == userId && r.Movie_id == movie.Id);

                        var isInRecommendations = await _context.Recommendations
                            .AnyAsync(r => r.User_id == userId && r.Movie_id == movie.Id);

                        if (!isInFavorites && !isInRatings && !isInRecommendations && !string.IsNullOrEmpty(movie.Plot))
                        {
                            _logger.LogInformation("Adding recommendation: {movieTitle}", movie.Title);

                            _context.Recommendations.Add(recommendation);
                            await _context.SaveChangesAsync();

                            _logger.LogInformation("Recommendation saved: {movieTitle}", movie.Title);
                        }
                        else
                        {
                            _logger.LogInformation(
                                "Movie already exists in favorites, ratings, or recommendations: {movieTitle}",
                                movie.Title);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Movie not found or error returned: {title}", title);
                    }

                    _logger.LogInformation("Finished processing movie: {title}", title);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing movies");
            }

            _logger.LogInformation("Finished processing all movies");
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
        var sb = new StringBuilder();
        sb.AppendLine("I rated the following movies:");

        foreach (var rating in ratings) sb.AppendLine($"- {rating.Movie_title}: {rating.Rating_value}");

        return sb.ToString();
    }

    private string GenerateFavoritesString(List<Favorite> favorites)
    {
        var sb = new StringBuilder();
        sb.AppendLine("My favorite movies are:");

        foreach (var favorite in favorites) sb.AppendLine($"- {favorite.Movie_title}");

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