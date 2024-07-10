using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Data;

namespace Project.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DataContext _context;

    public HomeController(ILogger<HomeController> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }
    
    public List<Rating> TopRated()
    {
        var movieIds = _context.Ratings.Select(r => r.Movie_id).Distinct().ToList();

        var aggregatedRatings = new List<Rating>();

        foreach (var movieId in movieIds)
        {
            var movieRatings = _context.Ratings.Where(r => r.Movie_id == movieId).ToList();

            if (movieRatings.Count == 1)
            {
                aggregatedRatings.Add(movieRatings[0]);
            }
            else if (movieRatings.Count > 1)
            {
                var averageRating = new Rating
                {
                    Movie_id = movieId,
                    Rating_value = Math.Round(movieRatings.Average(r => r.Rating_value),1),
                    Movie_title = movieRatings[0].Movie_title,
                    Movie_poster = movieRatings[0].Movie_poster,
                    Created_at = DateTime.Now,
                    Updated_at = DateTime.Now
                };
                aggregatedRatings.Add(averageRating);
            }
        }

        var topRated = aggregatedRatings
            .OrderByDescending(r => r.Rating_value)
            .Take(10)
            .ToList();

        return topRated;
    }

    public IActionResult Index()
    {
        ViewData["TopRated"] = TopRated();
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

