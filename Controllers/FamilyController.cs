using System.Security.Claims;
using Chefster.Common;
using Chefster.Models;
using Chefster.Services;
using Chefster.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Chefster.Controllers;

[Authorize]
[Route("api/family")]
[ApiController]
public class FamilyController(
    ConsiderationsService considerationsService,
    FamilyService familyService,
    MemberService memberService,
    JobService jobService
) : ControllerBase
{
    private readonly ConsiderationsService _considerationsService = considerationsService;
    private readonly FamilyService _familyService = familyService;
    private readonly MemberService _memberService = memberService;
    private readonly JobService _jobService = jobService;

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
        // create the new family
        var NewFamily = new FamilyModel
        {
            // these shouldn't  be null so we added a "!"
            Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!,
            Email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!,
            CreatedAt = DateTime.UtcNow,
            PhoneNumber = Family.PhoneNumber,
            FamilySize = Family.FamilySize,
            NumberOfBreakfastMeals = Family.NumberOfBreakfastMeals,
            NumberOfLunchMeals = Family.NumberOfLunchMeals,
            NumberOfDinnerMeals = Family.NumberOfDinnerMeals,
            GenerationDay = Family.GenerationDay,
            GenerationTime = Family.GenerationTime,
            TimeZone = Family.TimeZone,
        };

        // create family and job
        var created = _familyService.CreateFamily(NewFamily);
        if (created.Success)
        {
            _jobService.CreateorUpdateEmailJob(created.Data!.Id);
        }

        foreach (MemberViewModel Member in Family.Members)
        {
            // create the new member
            var NewMember = new MemberCreateDto
            {
                FamilyId = User
                    .Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                    ?.Value!,
                Name = Member.Name,
                Notes = Member.Notes
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
                            Type = ConsiderationsEnum.Restriction,
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
                            Type = ConsiderationsEnum.Goal,
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
                            Type = ConsiderationsEnum.Cuisine,
                            Value = c.Text
                        };

                    _considerationsService.CreateConsideration(cuisine);
                }
            }
        }

        // TODO: send confirmation email
        var model = new ThankYouViewModel
        {
            EmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!,
            GenerationDay = Family.GenerationDay,
            GenerationTime = Family.GenerationTime
        };

        return RedirectToAction("ThankYou", "Index", model);
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

        // once we updated successfully, not now update the job with new generation times
        _jobService.CreateorUpdateEmailJob(updated.Data!.Id);

        return Ok(updated.Data);
    }
}
