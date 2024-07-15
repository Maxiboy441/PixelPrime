using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Services
{
    public class RecommendationService
    {
        private readonly DataContext _context;
        private readonly AiApiService _aiApiService;
        private readonly MovieApiService _movieApiService;

        public RecommendationService(DataContext context, AiApiService aiApiService, MovieApiService movieApiService)
        {
            _context = context;
            _aiApiService = aiApiService;
            _movieApiService = movieApiService;
        }

        public async Task GetRecommendations(int userId)
        {
            List<Rating> liked = await GetLikedMovies(userId);
            List<Favorite> favorites = await GetFavorites(userId);

            string prompt1 = GenerateRatingsString(liked);
            string prompt2 = GenerateFavoritesString(favorites);

            string finalPrompt = $"{prompt1}\n{prompt2}";

            List<string> response = await _aiApiService.GenerateResponse(finalPrompt);

            DateTime fourDaysAgo = DateTime.Now.AddDays(-8);
            await _context.Recommendations
                .Where(r => r.User_id == userId && r.Created_at < fourDaysAgo)
                .ExecuteDeleteAsync();

            foreach (var title in response)
            {
                Movie movie = await _movieApiService.GetMovieByName(title);

                if (movie.Title != "error")
                {
                    Recommendation recommendation = new Recommendation
                    {
                        Movie_id = movie.Id,
                        User_id = userId,
                        Movie_title = movie.Title,
                        Movie_poster = movie.Poster,
                        Created_at = DateTime.Now,
                        Updated_at = DateTime.Now
                };
                    _context.Recommendations.Add(recommendation);
                    await _context.SaveChangesAsync();
                }
            }
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