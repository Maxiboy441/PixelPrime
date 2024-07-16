using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using System.Collections.Generic;
using System.Linq;
using Project.Data; 
using Xunit;


namespace Reviews.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly DataContext _context;

        public ReviewsController(DataContext context)
        {
            _context = context;
        }

        // GET: Reviews/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Content,Rating,MovieId")] Review review)
        {
            if (ModelState.IsValid)
            {
                review.Created_at = DateTime.Now;
                review.Updated_at = DateTime.Now;
                _context.Add(review);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(review);
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
        public IActionResult DeleteConfirmed(int id)
        {
            var review = _context.Reviews.Find(id);
            _context.Reviews.Remove(review);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.Id == id);
        }
    }
    /// <summary>
    /// TEST
    /// </summary>
    public class ReviewsControllerTests
    {
        private DbContextOptions<DataContext> GetInMemoryDbContextOptions()
        {
            return new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void Create_Post_ValidModel_AddsReview()
        {
            var options = GetInMemoryDbContextOptions();
            using (var context = new DataContext(options))
            {
                var controller = new ReviewsController(context);
                var review = new Review
                {

                    Text = "Film ist ass",
                    Movie_id = "1"
                };

                var result = controller.Create(review) as RedirectToActionResult;

                Assert.Equal("Index", result.ActionName);
                Assert.Single(context.Reviews);
            }
        }

        // Additional tests for Edit, Delete, etc.
    }
}






