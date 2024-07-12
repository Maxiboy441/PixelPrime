using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Controllers
{
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public MovieController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> StoreWatchlist(string movieId, string title, string poster)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var favorite = new Favorite
            {
                MovieId = movieId,
                Title = title,
                Poster = poster,
                UserId = user.Id
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Favorite added successfully." });
        }

        [HttpDelete]
        public async Task<IActionResult> DestroyWatchlist(int favoriteId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var favorite = _context.Favorites.FirstOrDefault(f => f.FavoriteId == favoriteId && f.UserId == user.Id);
            if (favorite == null)
            {
                return NotFound();
            }

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Favorite removed successfully." });
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
