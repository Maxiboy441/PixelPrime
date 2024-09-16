using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Project.Data;
using Project.Models;
using Project.Models.ViewModels;
using Project.Services;

namespace Project.Controllers
{
    public class MoviesController : Controller
    {
        private readonly DataContext _context;
        private readonly ILogger<MoviesController> _logger;
        private readonly CacheService _cache;
        private readonly MovieApiService _movieApiService;

        public MoviesController(CacheService cache, ILogger<MoviesController> logger, DataContext context, MovieApiService movieApiService)
        {
            _cache = cache;
            _logger = logger;
            _context = context;
            _movieApiService = movieApiService;
        }

        [HttpPost]
        public async Task<IActionResult> StoreFavoriteMovie(Favorite favorite, string movieId, string title, string poster)
        {
            var userJson = HttpContext.Session.GetString("CurrentUser");

            if (userJson != null)
            {
                var currentUser = JsonConvert.DeserializeObject<User>(userJson);

                favorite.User_id = currentUser.Id;
                favorite.Movie_id = movieId;
                favorite.Movie_title = title;
                favorite.Movie_poster = poster;
                favorite.Created_at = DateTime.Now;
                favorite.Updated_at = DateTime.Now;


                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"{title} has been successfully added to your favorites!";

                return Redirect(Request.Headers["Referer"].ToString());
            }
            else
            {
                var originalUrl = Request.Headers["Referer"].ToString();
                return RedirectToAction("Login", "Auth", new { returnUrl = originalUrl });

            }
        }

        [HttpPost]
        public async Task<IActionResult> DestroyFavoriteMovie(string id)
        {
            return await DeleteFavorite(id, "movie_page");
        }

        [HttpPost]
        public async Task<IActionResult> DestroyFavoriteMovieFromProfile(string id)
        {
            return await DeleteFavorite(id, "profile");
        }

        private async Task<IActionResult> DeleteFavorite(string id, string route)
        {
            var userJson = HttpContext.Session.GetString("CurrentUser");

            if (userJson != null)
            {
                var currentUser = JsonConvert.DeserializeObject<User>(userJson);
                var favorite = await _context.Favorites.FirstOrDefaultAsync(movie => movie.Movie_id == id && movie.User_id == currentUser.Id);

                if (favorite == null)
                {
                    TempData["FailMessage"] = "The content you are trying to remove does not exist in your favorites.";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();

                if(route == "profile")
                {
                    return Json(new { success = true, message = $"{favorite.Movie_title} has been removed from your favorites." });
                } else
                {
                    TempData["SuccessMessage"] = $"{favorite.Movie_title} has been removed from your favorites.";
                    return Redirect(Request.Headers["Referer"].ToString());
                }
            }
            else
            {
                if(route == "profile")
                {
                    return Json(new { success = false, redirectToLogin = true, message = "User not logged in." });
                } else
                {
                    var originalUrl = Request.Headers["Referer"].ToString();
                    return RedirectToAction("Login", "Auth", new { returnUrl = originalUrl });
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> StoreWatchlistMovie(Watchlist watchlist, string movieId, string title, string poster)
        {
            var userJson = HttpContext.Session.GetString("CurrentUser");

            if (userJson != null)
            {
                var currentUser = JsonConvert.DeserializeObject<User>(userJson);

                watchlist.User_id = currentUser.Id;
                watchlist.Movie_id = movieId;
                watchlist.Movie_title = title;
                watchlist.Movie_poster = poster;
                watchlist.Created_at = DateTime.Now;
                watchlist.Updated_at = DateTime.Now;


                _context.Watchlists.Add(watchlist);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"{title} has been successfully added to your watchlist!";

                return Redirect(Request.Headers["Referer"].ToString());
            }
            else
            {
                var originalUrl = Request.Headers["Referer"].ToString();
                return RedirectToAction("Login", "Auth", new { returnUrl = originalUrl });

            }
        }

        [HttpPost]
        public async Task<IActionResult> DestroyWatchlistMovie(string id)
        {
            return await DeleteWatchlist(id, "movie_page");
        }

        [HttpPost]
        public async Task<IActionResult> DestroyWatchlistMovieFromProfile(string id)
        {
            return await DeleteWatchlist(id, "profile");
        }

        private async Task<IActionResult> DeleteWatchlist (string id, string route)
        {
            var userJson = HttpContext.Session.GetString("CurrentUser");

            if (userJson != null)
            {
                var currentUser = JsonConvert.DeserializeObject<User>(userJson);

                var movie = await _context.Watchlists.FirstOrDefaultAsync(movie => movie.Movie_id == id && movie.User_id == currentUser.Id);

                if (movie == null)
                {
                    TempData["FailMessage"] = "The content you are trying to remove does not exist in your watchlist.";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                _context.Watchlists.Remove(movie);
                await _context.SaveChangesAsync();

                if(route == "profile")
                {
                    return Json(new { success = true, message = $"{movie.Movie_title} has been removed from your watchlist." });
                } else
                {
                    TempData["SuccessMessage"] = $"{movie.Movie_title} has been removed from your watchlist.";

                    return Redirect(Request.Headers["Referer"].ToString());
                }
            }
            else
            {
                if (route == "profile")
                {
                    return Json(new { success = false, redirectToLogin = true, message = "User not logged in." });
                } else
                {
                    var originalUrl = Request.Headers["Referer"].ToString();
                    return RedirectToAction("Login", "Auth", new { returnUrl = originalUrl });
                }
            }
        }

        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> Show(string id)
        {
            var movie = await _cache.GetContentAsync(id);

            if (movie == null)
            {
                return View("NotFound");
            }

            var reviews = await _context.Reviews
                .Where(review => review.Movie_id == id)
                .Include(review => review.User)
                .ToListAsync();
            
            var ratings = await _context.Ratings
                .Where(rating => rating.Movie_id == id)
                .Include(rating => rating.User)
                .ToListAsync();
            
            var reviewsWithRatings = reviews
                .Select(review => new ReviewWithRatings
                {
                    Review = review,
                    Rating = ratings.FirstOrDefault(rating => rating.User_id == review.User_id)?.Rating_value != null
                        ? (int?)Convert.ToInt32(ratings.FirstOrDefault(rating => rating.User_id == review.User_id)?.Rating_value)
                        : null
                })
                .ToList();
            
            var userJson = HttpContext.Session.GetString("CurrentUser");
            int userId = 0;
            bool isFavorite = false;
            bool isWatchlist = false;
            bool isRated = false;
            double currentUserRating = 0.0;
            bool userHasReview = false;

            if (userJson != null)
            {
                var currentUser = JsonConvert.DeserializeObject<User>(userJson);
                userId = currentUser.Id;

                isFavorite = await IsFavorite(userId, id);
                isWatchlist = await IsWatchlist(userId, id);
                isRated = await IsRated(userId, id);
                currentUserRating = (await GetUserRating(userId, id)) ?? 0.0;
                userHasReview = reviews.Any(r => r.User_id == userId);
            }

            var averageRating = await GetAverageRating(id);

            var viewModel = new MovieDetailsViewModel
            {
                Movie = movie,
                AverageRating = averageRating.HasValue ? averageRating.Value.ToString("0.0") : "",
                IsFavorite = isFavorite,
                IsWatchlist = isWatchlist,
                IsRated = isRated,
                HasAverageRating = !string.IsNullOrEmpty(averageRating.ToString()),
                UserHasRating = currentUserRating != 0.0,
                CurrentUserRating = currentUserRating == 0.0 ? string.Empty : currentUserRating.ToString("0.0"),
                CurrentUserId = userId,
                UserHasReview = userHasReview,
                ReviewWithRatings = reviewsWithRatings,
                MovieTrailer = await _movieApiService.GetTrailerByImdb(movie.Id),
            };

            return View(viewModel);
        }
        
        private async Task<bool> IsFavorite(int userId, string movieId)
        {
            return await _context.Favorites.AnyAsync(f => f.Movie_id == movieId && f.User_id == userId);
        }
        
        private async Task<bool> IsWatchlist(int userId, string movieId)
        {
            return await _context.Watchlists.AnyAsync(w => w.Movie_id == movieId && w.User_id == userId);
        }
        
        private async Task<bool> IsRated(int userId, string movieId)
        {
            return await _context.Ratings.AnyAsync(r => r.Movie_id == movieId && r.User_id == userId);
        }
        
        private async Task<double?> GetUserRating(int userId, string movieId)
        {
            return await _context.Ratings
                .Where(r => r.Movie_id == movieId && r.User_id == userId)
                .Select(r => r.Rating_value)
                .FirstOrDefaultAsync();
        }
        
        private async Task<double?> GetAverageRating(string movieId)
        {
            return await _context.Ratings
                .Where(r => r.Movie_id == movieId)
                .AverageAsync(r => (double?)r.Rating_value);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddRating(string movieId, string poster, string title, int ratingValue)
        {
            var userJson = HttpContext.Session.GetString("CurrentUser");

            if (!string.IsNullOrEmpty(userJson))
            {
                var currentUser = JsonConvert.DeserializeObject<User>(userJson);
                var existingRating = await _context.Ratings
                    .FirstOrDefaultAsync(r => r.Movie_id == movieId && r.User_id == currentUser.Id);

                if (existingRating != null)
                {
                    UpdateExistingRating(existingRating, ratingValue);
                }
                else
                {
                    await AddNewRating(movieId, poster, title, ratingValue, currentUser.Id);
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"You've successfully rated {title}!";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            else
            {
                var originalUrl = Request.Headers["Referer"].ToString();
                return RedirectToAction("Login", "Auth", new { returnUrl = originalUrl });

            }
        }

        private void UpdateExistingRating(Rating existingRating, int ratingValue)
        {
            existingRating.Rating_value = ratingValue;
            existingRating.Updated_at = DateTime.Now;
        }

        private async Task AddNewRating(string movieId, string poster, string title, int ratingValue, int userId)
        {
            var rating = new Rating
            {
                Movie_id = movieId,
                Movie_poster = poster,
                Movie_title = title,
                User_id = userId,
                Rating_value = ratingValue,
                Created_at = DateTime.Now,
                Updated_at = DateTime.Now
            };

            await _context.Ratings.AddAsync(rating);
        }
    }
}