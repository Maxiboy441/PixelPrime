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
        var topRatedMovies = _context.Ratings
            .GroupBy(r => new { r.Movie_id, r.Movie_title, r.Movie_poster })
            .Select(g => new Rating
            {
                Movie_id = g.Key.Movie_id,
                Movie_title = g.Key.Movie_title,
                Movie_poster = g.Key.Movie_poster,
                Rating_value = Math.Round(g.Average(r => r.Rating_value), 1),
                Created_at = DateTime.Now,
                Updated_at = DateTime.Now
            })
            .OrderByDescending(r => r.Rating_value)
            .Take(10)
            .ToList();

        return topRatedMovies;
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

