using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Newtonsoft.Json;
using static BCrypt.Net.BCrypt;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Services;
using Project.HostedServices;

namespace Project.Controllers
{
    public class AuthController : Controller
    {
        private readonly DataContext _context;
        private readonly RecommendationService _recommendationService;

        public AuthController(DataContext context, RecommendationService recommendationService)
        {
            _context = context;
            _recommendationService = recommendationService;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("CurrentUser")))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null || !Verify(model.Password, user.Password))
            {
                ModelState.AddModelError(string.Empty, "Email or password is wrong.");
                return View(model);
            }

            var userWithoutPassword = new User
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
            };
            string userJson = JsonConvert.SerializeObject(userWithoutPassword);
            HttpContext.Session.SetString("CurrentUser", userJson);
            
            var userId = userWithoutPassword.Id;
            var newestRecommendationDate = await _recommendationService.GetNewestRecommendationDate(userId);
            var favorites = await _recommendationService.GetFavorites(userId);
            var ratings = await _recommendationService.GetLikedMovies(userId);
            
            if ((favorites.Any() || ratings.Any()) &&
                (newestRecommendationDate == null || newestRecommendationDate < DateTime.Now.AddDays(-7)))
            {
                var backgroundService = HttpContext.RequestServices.GetRequiredService<BackgroundRecommendationService>();
                _ = backgroundService.RunRecommendationTask(userId);
            }

            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Index", "Home");
            }

            return Redirect(returnUrl);
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("CurrentUser")))
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(User user, string passwordConfirmation)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Email == user.Email);

            if (userExists)
            {
                ModelState.AddModelError("Email", "A user with this email already exists.");
            }

            if (user.Password != passwordConfirmation)
            {
                ModelState.AddModelError("PasswordConfirmation", "The password and confirmation password do not match.");
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            user.Password = HashPassword(user.Password);
            user.Created_at = DateTime.Now;
            user.Updated_at = DateTime.Now;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userWithoutPassword = new User
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
            };

            string userJson = JsonConvert.SerializeObject(userWithoutPassword);
            HttpContext.Session.SetString("CurrentUser", userJson);

            return RedirectToAction("Index", "Home");
        }


        public IActionResult Logout()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("CurrentUser")))
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Home");

            }
            return RedirectToAction("login", "Auth");
        }
    }
}