using Microsoft.AspNetCore.Mvc;

namespace Project.Controllers;

public class HowToUseController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}