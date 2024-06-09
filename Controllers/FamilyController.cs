using System.Security.Claims;
using Chefster.Models;
using Chefster.Services;
using Chefster.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Chefster.Controllers;

//[Authorize]
[Route("api/family")]
[ApiController]
public class FamilyController(
    ConsiderationsService considerationsService,
    FamilyService familyService,
    MemberService memberService
) : ControllerBase
{
    private readonly ConsiderationsService _considerationsService = considerationsService;
    private readonly FamilyService _familyService = familyService;
    private readonly MemberService _memberService = memberService;

    [HttpGet("{Id}")]
    public ActionResult<FamilyModel> GetFamily(string Id)
    {
        var family = _familyService.GetById(Id);

        if (family == null)
        {
            return NotFound(new { Message = $"No family found with familyId {Id}" });
        }

        return Ok(family.Data);
    }

    [HttpGet]
    public ActionResult<FamilyModel> GetAllFamilies()
    {
        var families = _familyService.GetAll();
        return Ok(families.Data);
    }

    [HttpPost]
    public IActionResult CreateFamily([FromForm] FamilyViewModel Family)
    {
        Console.WriteLine("Family created:");
        Console.WriteLine("Phone: " + Family.PhoneNumber);
        Console.WriteLine("Size: " + Family.FamilySize);
        Console.WriteLine("Day: " + Family.GenerationDay.ToString());
        Console.WriteLine("Time: " + Family.GenerationTime);
        Console.WriteLine("\nMembers:");
        foreach (MemberViewModel member in Family.Members)
        {
            Console.WriteLine("Name: " + member.Name);
            Console.Write("Restrictions: ");
            foreach (SelectListItem r in member.Restrictions)
            {
                if (r.Selected)
                {
                    Console.Write(r.Text + ", ");
                }
            }
            Console.Write("\nGoals: ");
            foreach (SelectListItem g in member.Goals)
            {
                if (g.Selected)
                {
                    Console.Write(g.Text + ", ");
                }
            }
            Console.Write("\nCuisines: ");
            foreach (SelectListItem c in member.Cuisines)
            {
                if (c.Selected)
                {
                    Console.Write(c.Text + ", ");
                }
            }
            Console.WriteLine("\n");
        }

        // create the new family
        FamilyModel NewFamily =
            new()
            {
                // these shouldn't  be null so we added a "!"
                Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!,
                Email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!,
                CreatedAt = DateTime.UtcNow,
                PhoneNumber = Family.PhoneNumber,
                FamilySize = Family.FamilySize,
                GenerationDay = Family.GenerationDay,
                GenerationTime = Family.GenerationTime
            };

        _familyService.CreateFamily(NewFamily);

        foreach (MemberViewModel Member in Family.Members)
        {
            // create the new member
            MemberCreateDto NewMember =
                new()
                {
                    FamilyId = User
                        .Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                        ?.Value!,
                    Name = Member.Name
                };

            MemberModel CreatedMember = _memberService.CreateMember(NewMember).Data!;

            // and their considerations
            foreach (SelectListItem r in Member.Restrictions)
            {
                if (r.Selected)
                {
                    // create a new consideration
                    ConsiderationsCreateDto restriction =
                        new()
                        {
                            MemberId = CreatedMember.MemberId,
                            Type = Enums.ConsiderationsEnum.Restriction,
                            Value = r.Text
                        };

                    _considerationsService.CreateConsideration(restriction);
                }
            }

            foreach (SelectListItem g in Member.Goals)
            {
                if (g.Selected)
                {
                    ConsiderationsCreateDto goal =
                        new()
                        {
                            MemberId = CreatedMember.MemberId,
                            Type = Enums.ConsiderationsEnum.Goal,
                            Value = g.Text
                        };

                    _considerationsService.CreateConsideration(goal);
                }
            }

            foreach (SelectListItem c in Member.Cuisines)
            {
                if (c.Selected)
                {
                    ConsiderationsCreateDto cuisine =
                        new()
                        {
                            MemberId = CreatedMember.MemberId,
                            Type = Enums.ConsiderationsEnum.Cuisine,
                            Value = c.Text
                        };

                    _considerationsService.CreateConsideration(cuisine);
                }
            }
        }

        return RedirectToAction("Index", "Chat");
    }

    [HttpDelete("{Id}")]
    public ActionResult DeleteFamily(string Id)
    {
        var deleted = _familyService.DeleteFamily(Id);

        if (!deleted.Success)
        {
            return BadRequest($"Error: {deleted.Error}");
        }

        return Ok();
    }

    [HttpPut("{Id}")]
    public ActionResult<FamilyModel> UpdateFamily(string Id, [FromBody] FamilyUpdateDto family)
    {
        var updated = _familyService.UpdateFamily(Id, family);

        if (!updated.Success)
        {
            return BadRequest($"Error: {updated.Error}");
        }

        return Ok(updated.Data);
    }
}
