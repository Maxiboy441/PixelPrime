using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Project.Data;
using Project.Models;
using Project.Services;

namespace Project.Controllers
{
    public class MoviesController : Controller
    {
        private readonly DataContext _context;
        private readonly MovieApiService _movieApiService;
        
        public MoviesController(DataContext context, MovieApiService movieApiService)
        {
            _context = context;
            _movieApiService = movieApiService;
        }

        [HttpPost]
        public async Task<IActionResult> StoreFavorite(Favorite favorite, string movieId, string title, string poster)
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

                TempData["SuccessMessage"] = "Movie successfully added to your favorites!";

                return Redirect(Request.Headers["Referer"].ToString());
            }
            else
            {
                var originalUrl = Request.Headers["Referer"].ToString();
                return RedirectToAction("Login", "Auth", new { returnUrl = originalUrl });

            }
        }

        [HttpPost]
        public async Task<IActionResult> DestroyFavorite(string favoriteId)
        {
            var userJson = HttpContext.Session.GetString("CurrentUser");
            
            if (userJson != null)
            {
                var currentUser = JsonConvert.DeserializeObject<User>(userJson);

                var favorite = await _context.Favorites.FirstOrDefaultAsync(movie => movie.Movie_id == favoriteId && movie.User_id == currentUser.Id);
                
                
                if (favorite == null)
                {
                    TempData["FailMessage"] = "The movie you are trying to remove does not exist in your favorites.";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"The movie '{favorite.Movie_title}' has been removed from your favorites.";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            else
            {
                var originalUrl = Request.Headers["Referer"].ToString();
                return RedirectToAction("Login", "Auth", new { returnUrl = originalUrl });
            }
        }

        [HttpPost]
        public async Task<IActionResult> StoreWatchlist(Watchlist watchlist, string movieId, string title, string poster)
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

                TempData["SuccessMessage"] = "Movie successfully added to your watchlist!";

                return Redirect(Request.Headers["Referer"].ToString());
            }
            else
            {
                var originalUrl = Request.Headers["Referer"].ToString();
                return RedirectToAction("Login", "Auth", new { returnUrl = originalUrl });

            }
        }

        [HttpPost]
        public async Task<IActionResult> DestroyWatchlist(string id)
        {
            var userJson = HttpContext.Session.GetString("CurrentUser");

            if (userJson != null)
            {
                var currentUser = JsonConvert.DeserializeObject<User>(userJson);

                var movie = await _context.Watchlists.FirstOrDefaultAsync(movie => movie.Movie_id == id && movie.User_id == currentUser.Id);

                if (movie == null)
                {
                    TempData["FailMessage"] = "The movie you are trying to remove does not exist in your watchlist.";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                _context.Watchlists.Remove(movie);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"The movie '{movie.Movie_title}' has been removed from your watchlist.";

                return Redirect(Request.Headers["Referer"].ToString());
            }
            else
            {
                var originalUrl = Request.Headers["Referer"].ToString();
                return RedirectToAction("Login", "Auth", new { returnUrl = originalUrl });
            }
        }
        
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> Show(string id)
        {
            var movie = await _movieApiService.GetMovieById(id);

            if (movie == null)
            {
                return View("NotFound");
            }

            var userJson = HttpContext.Session.GetString("CurrentUser");
            bool isFavorite = false;
            bool isWatchlist = false;
    
            if (userJson != null)
            {
                var currentUser = JsonConvert.DeserializeObject<User>(userJson);

                isFavorite = await _context.Favorites.AnyAsync(f => f.Movie_id == id && f.User_id == currentUser.Id);
                isWatchlist = await _context.Watchlists.AnyAsync(w => w.Movie_id == id && w.User_id == currentUser.Id);
            }

            var averageRating = await _context.Ratings
                .Where(r => r.Movie_id == id)
                .AverageAsync(r => (double?)r.Rating_value) ?? 0;

            var hasRating = await _context.Ratings.AnyAsync(r => r.Movie_id == id);

            ViewData["Movie"] = movie;
            ViewData["AverageRating"] = hasRating ? averageRating.ToString("0.0") : "This movie has not yet been rated";
            ViewData["IsFavorite"] = isFavorite;
            ViewData["IsWatchlist"] = isWatchlist;

            return View(movie);
        }

    
        [HttpPost]
        public async Task<IActionResult> AddRating(string movieId, string poster, string title, int ratingValue)
        {
            var userJson = HttpContext.Session.GetString("CurrentUser");

            if (userJson != null)
            {
                var currentUser = JsonConvert.DeserializeObject<User>(userJson);

                var existingRating = await _context.Ratings
                    .FirstOrDefaultAsync(r => r.Movie_id == movieId && r.User_id == currentUser.Id);

                if (existingRating != null)
                {
                    existingRating.Rating_value = ratingValue;
                    existingRating.Updated_at = DateTime.Now;
                }
                else
                {
                    
                    var rating = new Rating
                    {
                        
                        Movie_id = movieId,
                        Movie_poster = poster,
                        Movie_title = title,
                        User_id = currentUser.Id,
                        Rating_value = ratingValue,
                        Created_at = DateTime.Now,
                        Updated_at = DateTime.Now
                    };

                    _context.Ratings.Add(rating);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Show", new { id = movieId });
            }
            else
            {
                var originalUrl = Request.Headers["Referer"].ToString();
                return RedirectToAction("Login", "Auth", new { returnUrl = originalUrl });
            }
        }
    }
}
