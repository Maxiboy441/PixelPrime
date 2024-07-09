using System;
using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Newtonsoft.Json;
using static BCrypt.Net.BCrypt;
using Microsoft.EntityFrameworkCore;
using Project.Data;

namespace Project.Controllers
{
    public class AuthController : Controller
    {
        private readonly DataContext _context;

        public AuthController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                //ModelState.AddModelError("InvalidCredentials", "Invalid username or password.");

                return RedirectToAction("Login", "Auth");
            }

            if (Verify(password, user.Password))
            {
                //var userWithoutPassword = new User
                //{
                //    Id = user.Id,
                //    Name = user.Name,
                //    Email = user.Email,
                //};
                //string userJson = JsonConvert.SerializeObject(userWithoutPassword);
                //HttpContext.Session.SetString("CurrentUser", userJson);

                return RedirectToAction("index", "Home");
            }

            //ModelState.AddModelError("InvalidCredentials", "Invalid username or password.");

            return RedirectToAction("login", "Auth");

        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(User user, string passwordConfirmation)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Email == user.Email);

            if (userExists)
            {
                return RedirectToAction("SignUp", "Auth");
            }

            if (ModelState.IsValid && user.Password == passwordConfirmation)
            {
                user.Password = HashPassword(user.Password);
                user.Created_at = DateTime.Now;
                user.Updated_at = DateTime.Now;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                //var userWithoutPassword = new User
                //{
                //    Id = user.Id,
                //    Name = user.Name,
                //    Email = user.Email,
                //};

                //string userJson = JsonConvert.SerializeObject(userWithoutPassword);
                //HttpContext.Session.SetString("CurrentUser", userJson);

                //return Ok("User registered successfully");
                return RedirectToAction("index", "Home");
            }

            //    TempData["ErrorMessage"] = "User with this username already exists.";

            return RedirectToAction("SignUp", "Auth");
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