using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Services
{
    public class RecommendationService
    {
        private static DataContext _context;
        private static AiApiService _aiApiService;

        public RecommendationService(DataContext context, AiApiService aiApiService)
        {
            _context = context;
            _aiApiService = aiApiService;
        }

        public static async Task GetRecommendations(int User_id)
        {
            List<Rating> liked = await GetLikedMovies(User_id);
            List<Favorite> favorites = await GetFavorites(User_id);

            string prompt1 = GenerateRatingsString(liked);
            string prompt2 = GenerateFavoritesString(favorites);
    
            string finalPrompt = $"{prompt1}\n{prompt2}";
    
            List<string> response = await _aiApiService.GenerateResponse(finalPrompt);

            DateTime fourDaysAgo = DateTime.Now.AddDays(-8);
            await _context.Recommendations
                .Where(r => r.User_id == User_id && r.Created_at < fourDaysAgo)
                .ExecuteDeleteAsync();

            foreach (var title in response)
            {
                Movie movie = await MovieApiService.GetMovieByName(title);

                if (movie.Title != "error")
                {
                    Recommendation recommendation = new Recommendation
                    {
                        Movie_id = movie.Id,
                        User_id = User_id,
                        Movie_title = movie.Title,
                        Movie_poster = movie.Poster,
                    };
                    _context.Recommendations.Add(recommendation);
                }
            }

            await _context.SaveChangesAsync();
        }

        public static async Task<List<Rating>> GetLikedMovies(int User_id)
        {
            return await _context.Ratings
                .Where(r => r.User_id == User_id && r.Rating_value > 7.5)
                .OrderByDescending(r => r.Created_at)
                .Take(10)
                .ToListAsync();
        }
 
        public static async Task<List<Favorite>> GetFavorites(int User_id)
        {
            return await _context.Favorites
                .Where(r => r.User_id == User_id)
                .ToListAsync();
        }
 
        private static string GenerateRatingsString(List<Rating> ratings)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("I rated following movies as such:");

            foreach (var rating in ratings)
            {
                sb.AppendLine($"{rating.Movie_title}, {rating.Rating_value}");
            }

            return sb.ToString();
        }
 
        private static string GenerateFavoritesString(List<Favorite> ratings)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("My favorite movies of all times are");

            foreach (var rating in ratings)
            {
                sb.AppendLine($"{rating.Movie_title}");
            }

            return sb.ToString();
        }

        public static async Task<DateTime?> GetNewestRecommendationDate(int userId)
        {
            return await _context.Recommendations
                .Where(r => r.User_id == userId)
                .OrderByDescending(r => r.Created_at)
                .Select(r => r.Created_at)
                .FirstOrDefaultAsync();
        }
    }
}