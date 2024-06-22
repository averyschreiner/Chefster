using System.Diagnostics;
using System.Security.Claims;
using Chefster.Common;
using Chefster.Models;
using Chefster.Services;
using Chefster.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace Chefster.Controllers;

// use this to make swagger ignore this controller if its not really an api
[ApiExplorerSettings(IgnoreApi = true)]
public class IndexController(FamilyService familyService, MemberService memberService) : Controller
{
    private readonly FamilyService _familyService = familyService;
    private readonly MemberService _memberService = memberService;

    [Authorize]
    [Route("/chat")]
    public IActionResult Chat()
    {
        return View();
    }

    [Authorize]
    [HttpGet]
    [Route("/createprofile")]
    public IActionResult CreateProfile()
    {
        var id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
        var family = _familyService.GetById(id).Data;

        if (family == null)
        {
            var model = new FamilyViewModel
            {
                PhoneNumber = "",
                FamilySize = 1,
                NumberOfBreakfastMeals = 0,
                NumberOfLunchMeals = 0,
                NumberOfDinnerMeals = 7,
                GenerationDay = DayOfWeek.Sunday,
                GenerationTime = TimeSpan.Zero,
                TimeZone = "",
                Members = new List<MemberViewModel>
                {
                    new()
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
        else
        {
            return View("Profile");
        }
    }

    [Authorize]
    [HttpGet]
    [Route("/updateprofile")]
    public IActionResult UpdateProfile()
    {
        var model = new FamilyViewModel
        {
            PhoneNumber = "",
            FamilySize = 1,
            GenerationDay = DayOfWeek.Sunday,
            GenerationTime = TimeSpan.Zero,
            TimeZone = "",
            Members = new List<MemberViewModel>
            {
                new()
                {
                    Name = "",
                    Restrictions = ConsiderationsLists.RestrictionsList,
                    Goals = ConsiderationsLists.GoalsList,
                    Cuisines = ConsiderationsLists.CuisinesList
                }
            }
        };

        var id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        var family = _familyService.GetById(id!).Data;

        var viewModelMembers = new List<MemberViewModel>();

        if (family != null)
        {
            var members = _memberService.GetByFamilyId(family.Id).Data;

            // if (members != null)
            // {
            // foreach (var mem in members)
            // {
            //     var tooAdd = new MemberViewModel
            //     {
            //         Name = mem.Name,
            //         Restrictions = mem.
            //     }
            //     viewModelMembers.Add(mem)
            // }
            // }


            var populatedModel = new FamilyViewModel
            {
                PhoneNumber = family.PhoneNumber,
                FamilySize = family.FamilySize,
                GenerationDay = family.GenerationDay,
                GenerationTime = family.GenerationTime,
                TimeZone = family.TimeZone,
                Members = viewModelMembers // currently empty, gotta figure that out
            };
            return View(populatedModel);
        }
        else
        {
            Console.WriteLine("Family was null :(");
            return View("GenericError");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
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
