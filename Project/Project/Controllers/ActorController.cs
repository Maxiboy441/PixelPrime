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

        if (actor == null)
        {
            actor = await _actorApiService.GetAndSaveActorAsync(name);
        }

        return View(actor);
    }
    
}

