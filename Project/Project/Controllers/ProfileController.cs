using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Project.Data;
using Project.Models;

namespace Project.Controllers
{
    public class ProfileController : Controller
    {
        private readonly DataContext _context;

        public ProfileController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userJson = HttpContext.Session.GetString("CurrentUser");
            if (userJson != null)
            {
                var currentUser = JsonConvert.DeserializeObject<User>(userJson);

                var favorites = await _context.Favorites
                    .Where(movie => movie.User_id == currentUser.Id)
                    .ToListAsync();
                var watchlist = await _context.Watchlists
                   .Where(movie => movie.User_id == currentUser.Id)
                   .ToListAsync();
                var recommendation = await _context.Recommendations
                  .Where(movie => movie.User_id == currentUser.Id)
                  .ToListAsync();

                var combinedItems = new List<IMovieItem>();
                combinedItems.AddRange(favorites);

                var viewModel = new ProfileViewModel
                {
                    User = currentUser,
                    Favorites = favorites.Cast<IMovieItem>().ToList(),
                    Watchlist = watchlist.Cast<IMovieItem>().ToList(),
                    Recommendations = recommendation.Cast<IMovieItem>().ToList()
                };

                return View(viewModel);

            }
            return RedirectToAction("Login", "Auth");
        }
    }
}
