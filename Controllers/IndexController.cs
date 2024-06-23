using System.Diagnostics;
using System.Security.Claims;
using Chefster.Common;
using Chefster.Models;
using Chefster.Services;
using Chefster.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chefster.Controllers;

// use this to make swagger ignore this controller if its not really an api
[ApiExplorerSettings(IgnoreApi = true)]
public class IndexController(FamilyService familyService) : Controller
{
    private readonly FamilyService _familyService = familyService;

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
                TimeZone =  "",
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

    [Route("/email")]
    public IActionResult EmailTemplate()
    {
        var model = new GordonResponseModel
        {
            Notes = "Here are the notes regarding the recipes for this week",
            BreakfastRecipes = [
                new GordonResponseModel.Recipe
                {
                    DishName = "Bacon and Eggs",
                    PrepareTime = "20 minutes",
                    Servings = 2,
                    Ingredients = ["6 eggs", "6 slices of bacon", "salt", "1 banana", "2 tbsp peanut butter"],
                    Instructions = ["Scramble the eggs", "Put eggs in the pan to cook", "Fry bacon in the pan of 6 minutes, flip halfway through", "Slice up banana"]
                }
            ],
            LunchRecipes = [
                new GordonResponseModel.Recipe
                {
                    DishName = "Bacon and Eggs",
                    PrepareTime = "20 minutes",
                    Servings = 2,
                    Ingredients = ["6 eggs", "6 slices of bacon", "salt", "1 banana", "2 tbsp peanut butter"],
                    Instructions = ["Scramble the eggs", "Put eggs in the pan to cook", "Fry bacon in the pan of 6 minutes, flip halfway through", "Slice up banana"]
                },
                new GordonResponseModel.Recipe
                {
                    DishName = "Bacon and Eggs",
                    PrepareTime = "20 minutes",
                    Servings = 2,
                    Ingredients = ["6 eggs", "6 slices of bacon", "salt", "1 banana", "2 tbsp peanut butter"],
                    Instructions = ["Scramble the eggs", "Put eggs in the pan to cook", "Fry bacon in the pan of 6 minutes, flip halfway through", "Slice up banana"]
                }

            ],
            DinnerRecipes = [
                new GordonResponseModel.Recipe
                {
                    DishName = "Bacon and Eggs",
                    PrepareTime = "20 minutes",
                    Servings = 2,
                    Ingredients = ["6 eggs", "6 slices of bacon", "salt", "1 banana", "2 tbsp peanut butter"],
                    Instructions = ["Scramble the eggs", "Put eggs in the pan to cook", "Fry bacon in the pan of 6 minutes, flip halfway through", "Slice up banana"]
                },
                new GordonResponseModel.Recipe
                {
                    DishName = "Bacon and Eggs",
                    PrepareTime = "20 minutes",
                    Servings = 2,
                    Ingredients = ["6 eggs", "6 slices of bacon", "salt", "1 banana", "2 tbsp peanut butter"],
                    Instructions = ["Scramble the eggs", "Put eggs in the pan to cook", "Fry bacon in the pan of 6 minutes, flip halfway through", "Slice up banana"]
                },
                new GordonResponseModel.Recipe
                {
                    DishName = "Bacon and Eggs",
                    PrepareTime = "20 minutes",
                    Servings = 2,
                    Ingredients = ["6 eggs", "6 slices of bacon", "salt", "1 banana", "2 tbsp peanut butter"],
                    Instructions = ["Scramble the eggs", "Put eggs in the pan to cook", "Fry bacon in the pan of 6 minutes, flip halfway through", "Slice up banana"]
                }
            ],
            GroceryList = ["almond milk", "chia seeds", "maple syrup", "banana", "rolled oats", "blueberries", "spinach", "avocado", "tomato", "corn tortillas", "black beans", "cilantro", "lime", "green onions", "quinoa", "cucumber", "red bell pepper", "olive oil", "chicken breast", "soy sauce", "ginger", "garlic", "honey", "salmon fillet", "broccolini", "asparagus", "lemon", "spaghetti squash", "marinara sauce", "vegan mozzarella", "bell peppers", "onion", "cherry tomatoes", "zucchini", "mushrooms", "taco seasoning", "ground beef", "cheddar cheese", "gluten-free hamburger buns", "lettuce", "pickles", "barbecue sauce", "ribeye steak", "rosemary", "sweet potatoes", "green beans"]
        };
        return View(model);
    }
}
