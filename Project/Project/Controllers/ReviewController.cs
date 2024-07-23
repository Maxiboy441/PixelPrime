using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Project.Data;

namespace Reviews.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly DataContext _context;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(ILogger<ReviewsController> logger, DataContext context)
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

                // Check if the user has already submitted a review for the same movie
                var existingReview = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.User_id == userId && r.Movie_id == movieId);

                if (existingReview != null)
                {
                    // User has already submitted a review for this movie
                    TempData["ErrorMessage"] = "You have already submitted a review for this movie.";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                // User has not submitted a review for this movie, proceed to add the new review
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

        // GET: Reviews/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = _context.Reviews.Find(id);
            if (review == null)
            {
                return NotFound();
            }
            return View(review);
        }

        // POST: Reviews/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Content,Rating,MovieId")] Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    review.Updated_at = DateTime.Now;
                    _context.Update(review);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(review);
        }

        // GET: Reviews/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = _context.Reviews
                .FirstOrDefault(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.Id == id);
        }

        // Assuming you have an Index action method to list reviews
        public IActionResult Index()
        {
            var reviews = _context.Reviews.ToList();
            return View(reviews);
        }
    }



    /// <summary>
    /// TEST
    /// </summary>
    //public class ReviewsControllerTests
    //{
    //    private DbContextOptions<DataContext> GetInMemoryDbContextOptions()
    //    {
    //        return new DbContextOptionsBuilder<DataContext>()
    //            .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //            .Options;
    //    }

    //    [Fact]
    //    public void Create_Post_ValidModel_AddsReview()
    //    {
    //        var options = GetInMemoryDbContextOptions();
    //        using (var context = new DataContext(options))
    //        {
    //            var controller = new ReviewsController(context);
    //            var review = new Review
    //            {

    //                Text = "Film ist ass",
    //                Movie_id = "1"
    //            };

    //            var result = controller.Create(review) as RedirectToActionResult;

    //            Assert.Equal("Index", result.ActionName);
    //            Assert.Single(context.Reviews);
    //        }
    //    }

        // Additional tests for Edit, Delete, etc.
    //}
}






