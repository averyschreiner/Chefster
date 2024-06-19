using System.Text;
using Chefster.Models;
using Hangfire;
using Hangfire.Storage;

namespace Chefster.Services;

public class JobService(
    ConsiderationsService considerationsService,
    FamilyService familyService,
    EmailService emailService,
    GordonService gordonService
)
{
    private readonly ConsiderationsService _considerationService = considerationsService;
    private readonly FamilyService _familyService = familyService;
    private readonly EmailService _emailService = emailService;
    private readonly GordonService _gordonService = gordonService;

    /*
    The service is responsible for created and updating jobs that will
    gather gordons response and then send emails when the correct time comes
    */

    // Since hangfire has one function for creating and updating jobs this made the most sense
    public void CreateorUpdateEmailJob(string familyId)
    {
        var family = _familyService.GetById(familyId).Data;
        TimeZoneInfo timeZone;

        if (TimeZoneInfo.TryConvertIanaIdToWindowsId(family.TimeZone, out string windowsTimeZoneId))
        {
            Console.WriteLine(windowsTimeZoneId);
            timeZone = TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId);
        }
        else
        {
            timeZone = TimeZoneInfo.Utc;
        }
        
        var options = new RecurringJobOptions
        {
            TimeZone = timeZone
        };

        // create the job for the family
        if (family != null)
        {
            RecurringJob.AddOrUpdate(
                family.Id,
                () => GatherAndSendEmail(familyId),
                Cron.Weekly(
                    family.GenerationDay,
                    family.GenerationTime.Hours,
                    family.GenerationTime.Minutes
                ),
                options
            );
        }
    }

    public async Task GatherAndSendEmail(string familyId)
    {
        // grab family, get gordon response, build email
        var family = _familyService.GetById(familyId).Data;
        var builtRequest = BuildGordonRequest(familyId)!;
        var gordonResponse = await _gordonService.GetMessageResponse(builtRequest);
        var body = BuildEmail(gordonResponse.Data!);

        if (family != null && body != null)
        {
            _emailService.SendEmail(
                family.Email,
                "Chefster - Your weekly meal plan has arrived!",
                body
            );
        }
    }

    public string? BuildGordonRequest(string familyId)
    {
        var stringBuiler = new StringBuilder();
        var gordonConsiderations =
            "Build 7 total recipes. Here is a list of dietary considerations. The list follow the pattern of considerationType = considerationValue.\n";
        var considerations = _considerationService.GetAllFamilyConsiderations(familyId).Data;

        if (considerations == null)
        {
            return null;
        }

        // loop through considerations and add them to the message for gordon
        stringBuiler.Append(gordonConsiderations);
        foreach (var consideration in considerations)
        {
            stringBuiler.Append($"Type: {consideration.Type} = Value: {consideration.Value}\n");
        }

        return stringBuiler.ToString();
    }

    private static string? BuildEmail(GordonResponseModel response)
    {
        var final = "";
        var allIngredients = new List<string> { };
        var notes = "";
        var name = "";
        var ingredients = new List<string> { };
        var instructions = new List<string> { };
        var prepareTime = "";
        var servings = 0;

        var recipes = response.Response;
        if (recipes.Count == 0 || recipes == null)
        {
            return null;
        }

        foreach (var recipe in recipes)
        {
            allIngredients = recipe!.AllIngredients;
            notes = recipe!.Notes;
            foreach (var detailRecipe in recipe.Recipes)
            {
                name = detailRecipe.DishName;
                ingredients = detailRecipe.Ingredients;
                instructions = detailRecipe.Instructions;
                prepareTime = detailRecipe.PrepareTime;
                servings = detailRecipe.Servings;

                var formatted =
                    $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            background-color: #f9f9f9;
            color: #333;
            padding: 20px;
        }}
        .container {{
            max-width: 600px;
            margin: auto;
            background-color: #fff;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0,0,0,0.1);
        }}
        summary {{
            font-size: 1.2em;
            font-weight: bold;
            margin-bottom: 10px;
            cursor: pointer;
        }}
        details {{
            margin-bottom: 20px;
        }}
        h1 {{
            color: #333;
        }}
        h2 {{
            color: #666;
            border-bottom: 1px solid #ccc;
            padding-bottom: 5px;
        }}
        ul {{
            list-style-type: disc;
            margin-left: 20px;
        }}
        li {{
            margin-bottom: 5px;
        }}
        .notes {{
            margin-top: 20px;
            padding: 10px;
            background-color: #e7f3fe;
            border-left: 5px solid #2196F3;
        }}
        .thickLine {{
            border: none;
            height: 5px;
            background-color: #000;
            margin: 20px 0;
        }}
    </style>
</head>
<body>
    <div class=""container"">
            <h2>Dish Name: {name}</h2>
            <p><strong>Prepare Time:</strong> {prepareTime}</p>
            <p><strong>Serves:</strong> {servings}</p>

            <div class=""notes"">
                <h2>Dish Notes:</h2>
                <p>{notes}</p>
            </div>

            <h2>Detailed Ingredients:</h2>
            <ul>
                {string.Join("", ingredients.Select(ingredient => $"<li>{ingredient}</li>"))}
            </ul>

            <h2>Preparation Instructions:</h2>
            <ol>
                {string.Join("", instructions.Select(instruction => $"<li>{instruction}</li>"))}
            </ol>
            <hr class=""thickLine"">
    </div>
</body>
</html>
";
                final += formatted;
            }
        }
        var header =
            $@"
                <!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            background-color: #f9f9f9;
            color: #333;
            padding: 20px;
        }}
        .container {{
            max-width: 600px;
            margin: auto;
            background-color: #fff;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0,0,0,0.1);
        }}
        h1 {{
            color: #333;
        }}
        h2 {{
            color: #666;
            border-bottom: 1px solid #ccc;
            padding-bottom: 5px;
        }}
        ul {{
            list-style-type: disc;
            margin-left: 20px;
        }}
        li {{
            margin-bottom: 5px;
            font-size: 14px; 
        }}
        .thickLine {{
            border: none;
            height: 5px;
            background-color: #000;
            margin: 20px 0;
        }}
        .content {{
            margin-top: 10px;
            padding: 10px;
            border: 1px solid #ccc;
            border-radius: 5px;
            background-color: #f9f9f9;
        }}
        table {{
            width: 100%;
            border-collapse: collapse;
        }}
        td {{
            padding: 5px;
            vertical-align: top;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <h1>Here are your meals for the week! Chef's Kiss ;)</h1>
        <h2>Your Grocery List:</h2>
        <div class=""content"">
            <table>
                <tr>
                    <td>
                        <ul>
                            {string.Join("", allIngredients.Take(allIngredients.Count / 2 + 1).Select(ingredient => $"<li>{ingredient}</li>"))}
                        </ul>
                    </td>
                    <td>
                        <ul>
                            {string.Join("", allIngredients.Skip(allIngredients.Count / 2 + 1).Select(ingredient => $"<li>{ingredient}</li>"))}
                        </ul>
                    </td>
                </tr>
            </table>
        </div>
        <hr class=""thickLine"">
    </div>
</body>
</html>
";
        return header + final;
    }
}
