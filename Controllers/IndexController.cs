using Chefster.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Security.Claims;

namespace Chefster.Controllers;

// use this to make swagger ignore this controller if its not really an api
[ApiExplorerSettings(IgnoreApi = true)]
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

    // [Authorize]
    [HttpGet]
    [Route("/createprofile")]
    public IActionResult CreateProfile()
    {
        foreach (Claim claim in User.Claims)
        {
            Console.WriteLine(claim.Type + ": " + claim.Value);
        }
        Console.WriteLine(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
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

    [HttpGet]
    [Route("/memberform")]
    public IActionResult MemberForm(int index)
    {
        return PartialView("MemberForm", index);
    }
}
