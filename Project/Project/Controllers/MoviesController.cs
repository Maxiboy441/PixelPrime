using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Project.Data;
using Project.Models;

namespace Project.Controllers
{
    public class MoviesController : Controller
    {
        private readonly DataContext _context;

        public MoviesController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> StoreWatchlist(string movieId, string title, string poster)
        {
            var userJson = HttpContext.Session.GetString("CurrentUser");

            if (userJson != null)
            {
                var currentUser = JsonConvert.DeserializeObject<User>(userJson);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == currentUser.Id);

                var favorite = new Favorite
                {
                    Movie_id = movieId,
                    Movie_title = title,
                    Movie_poster = poster,
                    User_id = user.Id
                };

                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Favorite added successfully." });
            }
            else
            {
                return Unauthorized();

            }
        }

        //[HttpDelete]
        //public async Task<IActionResult> DestroyWatchlist(int favoriteId)
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null)
        //    {
        //        return Unauthorized();
        //    }

        //    var favorite = _context.Favorites.FirstOrDefault(f => f.FavoriteId == favoriteId && f.UserId == user.Id);
        //    if (favorite == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Favorites.Remove(favorite);
        //    await _context.SaveChangesAsync();

        //    return Ok(new { message = "Favorite removed successfully." });
        //}
        public IActionResult Index()
        {
            return View();
        }
    }
}
