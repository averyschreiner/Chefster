using System.Security.Claims;
using Chefster.Common;
using Chefster.Models;
using Chefster.Services;
using Chefster.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson;

namespace Chefster.Controllers;

[Authorize]
[Route("api/family")]
[ApiController]
public class FamilyController(
    ConsiderationsService considerationsService,
    EmailService emailService,
    FamilyService familyService,
    MemberService memberService,
    JobService jobService,
    ViewToStringService viewToStringService
) : ControllerBase
{
    private readonly ConsiderationsService _considerationsService = considerationsService;
    private readonly EmailService _emailService = emailService;
    private readonly FamilyService _familyService = familyService;
    private readonly MemberService _memberService = memberService;
    private readonly JobService _jobService = jobService;
    private readonly ViewToStringService _viewToStringService = viewToStringService;

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
    public async Task<ActionResult> CreateFamily([FromForm] FamilyViewModel Family)
    {
        var familyId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;

        if (familyId == null)
        {
            return BadRequest("FamilyId was null when creating family");
        }
        // create the new family
        var NewFamily = new FamilyModel
        {
            // these shouldn't  be null so we added a "!"
            Id = familyId,
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

        // create all members and considerations for family
        await CreateMembersAndConsiderations(Family);

        // TODO: send confirmation email
        var body = await _viewToStringService.ViewToStringAsync(
            "ConfirmationEmail",
            new { FamilyId = NewFamily.Id }
        );
        _emailService.SendEmail(NewFamily.Email, "Thanks for signing up for Chefster!", body);

        var model = new ThankYouViewModel
        {
            EmailAddress = NewFamily.Email,
            GenerationDay = NewFamily.GenerationDay,
            GenerationTime = NewFamily.GenerationTime
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

    // this function is specificly for updating through a form since forms only support POST and PUT
    [HttpPost("/api/update/family")]
    public async Task<ActionResult<FamilyModel>> PostUpdateFamily(
        [FromForm] FamilyUpdateViewModel family
    )
    {
        var familyId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (familyId == null)
        {
            return Unauthorized("No Authorized User. Denied");
        }

        var updatedFamily = new FamilyUpdateDto
        {
            PhoneNumber = family.PhoneNumber,
            FamilySize = family.FamilySize,
            NumberOfBreakfastMeals = family.NumberOfBreakfastMeals,
            NumberOfLunchMeals = family.NumberOfLunchMeals,
            NumberOfDinnerMeals = family.NumberOfDinnerMeals,
            GenerationDay = family.GenerationDay,
            GenerationTime = family.GenerationTime,
            TimeZone = family.TimeZone,
        };

        var updated = _familyService.UpdateFamily(familyId, updatedFamily);

        Console.WriteLine("WE SHOULD HAVE UPDATED");

        if (!updated.Success)
        {
            return BadRequest($"Error: {updated.Error}");
        }

        // once we updated successfully, not now update the job with new generation times
        _jobService.CreateorUpdateEmailJob(updated.Data!.Id);

        // Update old members and create new considerations
        await UpdateOrCreateMembersAndCreateConsiderations(familyId, family);

        // probably redirect to summary page
        return RedirectToAction("Index", "Profile");
    }

    private Task CreateMembersAndConsiderations(FamilyViewModel Family)
    {
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
        return Task.CompletedTask;
    }

    private Task UpdateOrCreateMembersAndCreateConsiderations(
        string familyId,
        FamilyUpdateViewModel Family
    )
    {
        var dbMembers = _memberService.GetByFamilyId(familyId).Data;

        // if the member is present in db but not on update payload, delete it
        // handles the case of changing the family size
        if (dbMembers != null && dbMembers.Count > Family.Members.Count)
        {
            foreach (MemberModel dbm in dbMembers)
            {
                if (!Family.Members.Any(newM => newM.MemberId == dbm.MemberId))
                {
                    Console.WriteLine($"Deleting Member For Update: {dbm.ToJson()}");
                    _memberService.DeleteMember(dbm.MemberId);

                    var memConsiderations = _considerationsService.GetMemberConsiderations(
                        dbm.MemberId
                    );
                    if (memConsiderations.Data == null)
                    {
                        return Task.FromException(
                            new Exception(
                                $"Considerations was not just empty but null. Error: {memConsiderations.Error}"
                            )
                        );
                    }
                    foreach (ConsiderationsModel con in memConsiderations.Data)
                    {
                        _considerationsService.DeleteConsideration(con.ConsiderationId);
                    }
                }
            }
        }

        foreach (MemberUpdateViewModel Member in Family.Members)
        {
            MemberModel? contextMember = null;

            if (Member.ShouldDelete)
            {
                if (Member.MemberId != null)
                {
                    _memberService.DeleteMember(Member.MemberId);
                    DeleteOldConsiderations(Member.MemberId);
                }
            }
            else
            {
                // if member exists update it
                if (Member.MemberId != null)
                {
                    var UpdatedMember = new MemberUpdateDto
                    {
                        Name = Member.Name,
                        Notes = Member.Notes,
                    };

                    var updated = _memberService.UpdateMember(Member.MemberId, UpdatedMember);
                    if (!updated.Success)
                    {
                        return Task.FromException(
                            new Exception(
                                $"Failed to update member {UpdatedMember.ToJson()}. Error: {updated.Error}"
                            )
                        );
                    }
                    DeleteOldConsiderations(Member.MemberId);
                    contextMember = updated.Data!;
                }
                else
                {
                    // create the new member because the ID was null
                    var NewMember = new MemberCreateDto
                    {
                        FamilyId = familyId,
                        Name = Member.Name,
                        Notes = Member.Notes
                    };
                    var created = _memberService.CreateMember(NewMember);

                    if (!created.Success)
                    {
                        return Task.FromException(
                            new Exception(
                                $"Failed to create member. Member: {created.Error}. Error: {created.Error}"
                            )
                        );
                    }

                    contextMember = created.Data!;
                }
            }

            // Create all new considerations for update
            foreach (SelectListItem r in Member.Restrictions)
            {
                if (r.Selected && contextMember != null)
                {
                    ConsiderationsCreateDto restriction =
                        new()
                        {
                            MemberId = contextMember.MemberId,
                            Type = ConsiderationsEnum.Restriction,
                            Value = r.Text
                        };
                    var created = _considerationsService.CreateConsideration(restriction);
                    if (!created.Success)
                    {
                        return Task.FromException(
                            new Exception($"Error creating consideration. Error: {created.Error}")
                        );
                    }
                }
            }

            foreach (SelectListItem g in Member.Goals)
            {
                if (g.Selected && contextMember != null)
                {
                    ConsiderationsCreateDto goal =
                        new()
                        {
                            MemberId = contextMember.MemberId,
                            Type = ConsiderationsEnum.Goal,
                            Value = g.Text
                        };
                    var created = _considerationsService.CreateConsideration(goal);
                    if (!created.Success)
                    {
                        return Task.FromException(
                            new Exception($"Error creating consideration. Error: {created.Error}")
                        );
                    }
                }
            }

            foreach (SelectListItem c in Member.Cuisines)
            {
                if (c.Selected && contextMember != null)
                {
                    ConsiderationsCreateDto cuisine =
                        new()
                        {
                            MemberId = contextMember.MemberId,
                            Type = ConsiderationsEnum.Cuisine,
                            Value = c.Text
                        };
                    var created = _considerationsService.CreateConsideration(cuisine);
                    if (!created.Success)
                    {
                        return Task.FromException(
                            new Exception($"Error creating consideration. Error: {created.Error}")
                        );
                    }
                }
            }
            contextMember = null;
        }

        return Task.CompletedTask;
    }

    // Handles the deletion of considerations if a member was to update theirs
    private Task DeleteOldConsiderations(string memberId)
    {
        var considerations = _considerationsService.GetMemberConsiderations(memberId).Data;

        if (considerations != null)
        {
            var timeFrame = DateTime.UtcNow.AddSeconds(-10);
            foreach (var consideration in considerations)
            {
                if (consideration.CreatedAt <= timeFrame)
                {
                    var deleted = _considerationsService.DeleteConsideration(
                        consideration.ConsiderationId
                    );

                    Console.WriteLine($"Deleting this consideration: {deleted.Data.ToJson()}");

                    if (!deleted.Success)
                    {
                        return Task.FromException(
                            new Exception("Failed to delete old consideration")
                        );
                    }
                }
            }
        }

        return Task.CompletedTask;
    }
}
