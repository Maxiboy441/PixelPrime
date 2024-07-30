using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Models.ViewModels;
using Project.Data;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using static BCrypt.Net.BCrypt;

namespace Project.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly DataContext _context;

    public UserController(ILogger<UserController> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public IActionResult Update()
    {
        var userJson = HttpContext.Session.GetString("CurrentUser");

        if (!string.IsNullOrEmpty(userJson))
        {
            var currentUser = JsonConvert.DeserializeObject<User>(userJson);
            ViewBag.UserName = currentUser.Name;
            ViewBag.UserEmail = currentUser.Email;
            return View();
        }

        return RedirectToAction("Login", "Auth");
    }

    [HttpPost]
    public async Task<IActionResult> Update(AccountViewModel model)
    {
        var userJson = HttpContext.Session.GetString("CurrentUser");

        if (userJson != null)
        {
            var currentUser = JsonConvert.DeserializeObject<User>(userJson);

            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == currentUser.Id);

            if (!ModelState.IsValid)
            {
                ViewBag.UserName = currentUser.Name;
                ViewBag.UserEmail = currentUser.Email;
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.Name))
            {
                user.Name = model.Name;
            }

            if (!string.IsNullOrEmpty(model.Email))
            {
                var userExists = await _context.Users.AnyAsync(u => u.Email == model.Email);

                if (userExists)
                {
                    ModelState.AddModelError("Email", "A user with this email already exists.");
                    ViewBag.UserName = currentUser.Name;
                    ViewBag.UserEmail = currentUser.Email;
                    return View(model);
                }

                user.Email = model.Email;
            }

            if (!string.IsNullOrEmpty(model.Password))
            {
                if (model.Password != model.PasswordConfirmation)
                {
                    ModelState.AddModelError("PasswordConfirmation", "The password and confirmation password do not match.");
                    ViewBag.UserName = currentUser.Name;
                    ViewBag.UserEmail = currentUser.Email;
                    return View(model);
                }

                user.Password = HashPassword(model.Password);
            }

            user.Updated_at = DateTime.Now;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            var userWithoutPassword = new User
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
            };

            HttpContext.Session.SetString("CurrentUser", JsonConvert.SerializeObject(userWithoutPassword));

            TempData["SuccessMessage"] = "Account updated successfully";

            return RedirectToAction("Update", "User");

        }
        return RedirectToAction("Login", "Auth");
    }

    [HttpPost]
    [HttpPost]
    public async Task<IActionResult> Delete()
    {
        var userJson = HttpContext.Session.GetString("CurrentUser");

        if (userJson == null)
        {
            var originalUrl = Request.Headers["Referer"].ToString();
            return RedirectToAction("Login", "Auth", new { returnUrl = originalUrl });
        }

        var currentUser = JsonConvert.DeserializeObject<User>(userJson);
        if (currentUser == null)
        {
            return View("NotFound");
        }

        // Start a transaction
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == currentUser.Id);

                if (user == null)
                {
                    return View("NotFound");
                }

                // Remove user-related data
                _context.Users.Remove(user);

                // Commit transaction
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Clear session
                HttpContext.Session.Clear();

                // Log the successful deletion
                _logger.LogInformation($"User {currentUser.Id} deleted successfully.");

                TempData["SuccessMessage"] = "Account successfully deleted!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Rollback transaction in case of error
                await transaction.RollbackAsync();

                // Log the error
                _logger.LogError(ex, $"Error deleting user {currentUser.Id}.");

                // Return an error view or message
                TempData["ErrorMessage"] = "An error occurred while deleting the account.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}

