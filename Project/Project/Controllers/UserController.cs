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
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("CurrentUser")))
        {
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
                    return View(model);
                }

                user.Email = model.Email;
            }

            if (!string.IsNullOrEmpty(model.Password))
            {
                if (model.Password != model.PasswordConfirmation)
                {
                    ModelState.AddModelError("PasswordConfirmation", "The password and confirmation password do not match.");
                    return View(model);
                }

                user.Password = HashPassword(model.Password);
            }

            user.Updated_at = DateTime.Now;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Account updated successfully";

            return RedirectToAction("Update", "User");

        }
        return RedirectToAction("Login", "Auth");
    }
}

