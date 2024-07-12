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

                return Redirect(Request.Headers["Referer"].ToString());
            }
            else
            {
                return RedirectToAction("Login", "Auth");

            }
        }

        [HttpPost]
        public async Task<IActionResult> DestroyFavorite(int favoriteId)
        {
            var userJson = HttpContext.Session.GetString("CurrentUser");
            
            if (userJson != null)
            {
                var currentUser = JsonConvert.DeserializeObject<User>(userJson);

                var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.Id == favoriteId);

                if (favorite == null)
                {
                    return View("NotFound");
                }

                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();

                return Redirect(Request.Headers["Referer"].ToString());
            }
            else
            {
                return RedirectToAction("Login", "Auth");
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

                return Redirect(Request.Headers["Referer"].ToString());
            }
            else
            {
                return RedirectToAction("Login", "Auth");

            }
        }

        [HttpPost]
        public async Task<IActionResult> DestroyWatchlist(int id)
        {
            var userJson = HttpContext.Session.GetString("CurrentUser");

            if (userJson != null)
            {
                var currentUser = JsonConvert.DeserializeObject<User>(userJson);

                var movie = await _context.Watchlists.FirstOrDefaultAsync(f => f.Id == id);

                if (movie == null)
                {
                    return View("NotFound");
                }

                _context.Watchlists.Remove(movie);
                await _context.SaveChangesAsync();

                return Redirect(Request.Headers["Referer"].ToString());
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Show(string? name)
        {
            var movie = await _movieApiService.GetMovieByName(name);
            return View(movie);
        }
    }
}
