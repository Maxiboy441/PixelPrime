using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using Newtonsoft.Json;
using Project.Data;

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
        
        [HttpPost]
        public async Task<IActionResult> DestroyReviewFromMoviePage(string movieId)
        {
            return await Delete(movieId, "movie_page");
        }

        [HttpPost]
        public async Task<IActionResult> DestroyReviewFromProfile(string movieId)
        {
            return await Delete(movieId, "profile");
        }

        public async Task<IActionResult> Delete(string movieId,  string route)
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
                
                if(route == "profile")
                {
                    return Json(new { success = true, message = "Review has been successfully deleted" });
                } else
                {
                    TempData["SuccessMessage"] = "Review has been successfully deleted.";
                    var refererUrl = Request.Headers["Referer"].ToString();
                    var uri = new Uri(refererUrl, UriKind.RelativeOrAbsolute);
                    if (!uri.IsAbsoluteUri || uri.Host == "yourdomain.com")
                    {
                        return Redirect(refererUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            else
            {
                if(route == "profile")
                {
                    return Json(new { success = false, redirectToLogin = true, message = "User not logged in." });
                } else
                {
                    var refererUrl = Request.Headers["Referer"].ToString();
                    var uri = new Uri(refererUrl, UriKind.RelativeOrAbsolute);
                    var returnUrl = (!uri.IsAbsoluteUri || uri.Host == "yourdomain.com") ? refererUrl : Url.Action("Index", "Home");
                    return RedirectToAction("Login", "Auth", new { returnUrl = returnUrl });
                }
            }
        }
    }
}






