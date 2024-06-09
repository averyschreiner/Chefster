using Chefster.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Chefster.ViewModels;
using Chefster.Common;

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
        var model = new FamilyViewModel
        {
            PhoneNumber = "",
            FamilySize = 1,
            GenerationDay = DayOfWeek.Sunday,
            GenerationTime = TimeSpan.Zero,
            Members = new List<MemberViewModel>
            {
                new MemberViewModel
                {
                    Name = "",
                    Restrictions = ConsiderationsLists.RestrictionsList,
                    Goals = ConsiderationsLists.GoalsList,
                    Cuisines = ConsiderationsLists.CuisinesList
                }
            }
        };
        return View(model);
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
        var model = new MemberViewModel
        {
            Name = "",
            Index = index,
            Restrictions = ConsiderationsLists.RestrictionsList,
            Goals = ConsiderationsLists.GoalsList,
            Cuisines = ConsiderationsLists.CuisinesList
        };

        return PartialView("MemberForm", model);
    }
}
