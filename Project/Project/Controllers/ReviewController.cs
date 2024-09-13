using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using Newtonsoft.Json;
using Project.Data;
using System.Web;

namespace Reviews.Controllers
{
    public class ReviewController : Controller
    {
        private readonly DataContext _context;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(ILogger<ReviewController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(Review review, string movieId, string reviewTitle, string reviewText, string movieTitle, string moviePoster)
        {
            var userJson = HttpContext.Session.GetString("CurrentUser");

            if (userJson != null)
            {
                var currentUser = JsonConvert.DeserializeObject<User>(userJson);
                var userId = currentUser.Id;

                var existingReview = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.User_id == userId && r.Movie_id == movieId);

                if (existingReview != null)
                {
                    TempData["FailMessage"] = $"You have already submitted a review for {movieTitle}.";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                review.Title = reviewTitle;
                review.Text = reviewText;
                review.User_id = userId;
                review.Movie_id = movieId;
                review.Movie_title = movieTitle;
                review.Movie_poster = moviePoster;
                review.Created_at = DateTime.Now;
                review.Updated_at = DateTime.Now;
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Review successfully added!";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            else
            {
                var originalUrl = Request.Headers["Referer"].ToString();
                return RedirectToAction("Login", "Auth", new { returnUrl = originalUrl });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Review? review, string movieId, string reviewTitle, string reviewText)
        {
            var userJson = HttpContext.Session.GetString("CurrentUser");
            var currentUser = JsonConvert.DeserializeObject<User>(userJson);
            var userId = currentUser.Id;
            review = _context.Reviews.FirstOrDefault(review => review.Movie_id == movieId && review.User_id == currentUser.Id);

            if (review == null)
            {
                TempData["FailMessage"] = "Review not found!";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            if (userJson != null)
            {
                review.Title = reviewTitle;
                review.Text = reviewText;
                review.Updated_at = DateTime.Now;
                _context.Reviews.Update(review);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Review successfully updated!";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            else
            {
                var originalUrl = Request.Headers["Referer"].ToString();
                return RedirectToAction("Login", "Auth", new { returnUrl = originalUrl });
            }
        }

        public async Task<IActionResult> Delete(string movieId)
        {
            var userJson = HttpContext.Session.GetString("CurrentUser");

            if (userJson != null)
            {
                var currentUser = JsonConvert.DeserializeObject<User>(userJson);

                if (movieId == null)
                {
                    return View("NotFound");
                }

                var review = _context.Reviews.FirstOrDefault(review => review.Movie_id == movieId && review.User_id == currentUser.Id);

                if (review == null)
                {
                    return View("NotFound");
                }

                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Review successfully deleted!";

                var refererUrl = Request.Headers["Referer"].ToString();

                if (IsValidRedirectUrl(refererUrl))
                {
                    return Redirect(refererUrl); // Safe redirect
                }
                else
                {
                    return RedirectToAction("Index", "Home"); // Fallback if the URL is invalid
                }
            }
            else
            {
                var originalUrl = Request.Headers["Referer"].ToString();

                if (IsValidRedirectUrl(originalUrl))
                {
                    return RedirectToAction("Login", "Auth", new { returnUrl = originalUrl });
                }
                else
                {
                    return RedirectToAction("Login", "Auth", new { returnUrl = "/Home/Index" }); // Fallback to home if invalid
                }
            }
        }

        // Helper method to validate the URL
        private bool IsValidRedirectUrl(string urlString)
        {
            if (string.IsNullOrEmpty(urlString))
            {
                return false;
            }

            Uri url;
            bool isUriValid = Uri.TryCreate(urlString, UriKind.RelativeOrAbsolute, out url);

            if (!isUriValid)
            {
                return false; // Invalid URL
            }

            // Check if the URL is relative (internal to the application)
            if (!url.IsAbsoluteUri)
            {
                return true; // Safe, as it's relative
            }

            // If the URL is absolute, ensure it's pointing to a trusted host
            var trustedHosts = new List<string> { "example.org", "trustedsite.com" }; // Add your trusted hosts here
            return trustedHosts.Contains(url.Host);
        }


    }
}






