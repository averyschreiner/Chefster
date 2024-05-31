using Chefster.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace Chefster.Controllers;

public class IndexController : Controller
{
    private readonly ILogger<IndexController> _logger;

    public IndexController(ILogger<IndexController> logger)
    {
        _logger = logger;
    }

    [Authorize]
    [Route("/chat")]
    public IActionResult Chat()
    {
        return View();
    }

    [Authorize]
    public IActionResult CreateProfile()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Index()
    {
        return View();
    }

    [Route("/privacy")]
    public IActionResult Privacy()
    {
        return View();
    }

    [Authorize]
    [Route("/profile")]
    public IActionResult Profile()
    {
        return View();
    }
}
