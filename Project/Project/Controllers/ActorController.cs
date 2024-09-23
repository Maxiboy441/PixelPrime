using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Data;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Project.Services;
using static BCrypt.Net.BCrypt;

namespace Project.Controllers;

public class ActorController : Controller
{
    private readonly ILogger<ActorController> _logger;
    private readonly DataContext _context;
    private readonly ActorAPIService _actorApiService;

    public ActorController(ILogger<ActorController> logger, DataContext context, ActorAPIService actorApiService)
    {
        _logger = logger;
        _context = context;
        _actorApiService = actorApiService;
    }

    [HttpGet]
    public async Task<IActionResult> Show(string name)
    {
        var actor = await _context.Actors.FirstOrDefaultAsync(a => a.Name == name);

        if (actor != null)
        {
            bool accessible = await IsUrlAccessibleAsync(actor.Image);
            if (!accessible)
            {
                var service = new WikipediaMediaAPIService();
                var imageUrl = await service.GetFirstImageUrlAsync(actor.Name);
                actor.Image = imageUrl;
                await _context.SaveChangesAsync();
            }
        }

        if (actor == null)
        {
            actor = await _actorApiService.GetAndSaveActorAsync(name);
        }

        if (actor is null)        {
            return View("NotFound");
        }

        return View(actor);
    }
    
    public static async Task<bool> IsUrlAccessibleAsync(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                return response.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false; // Return false if there's an error
            }
        }
    }
}

