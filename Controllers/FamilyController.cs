using Chefster.Models;
using Chefster.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chefster.Controllers;

//[Authorize]
[Route("api/family")]
[ApiController]
public class FamilyController(FamilyService familyService) : ControllerBase
{
    private readonly FamilyService _familyService = familyService;

    [HttpGet("{Id}")]
    public ActionResult<FamilyModel> GetFamily(string Id)
    {
        var family = _familyService.GetById(Id);

        if (family.Data != null)
        {
            family.Data.Members = _familyService.GetMembers(Id).Data!;
            family.Data.WeeklyNotes = _familyService.GetNotes(Id).Data!;
        }

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
    public ActionResult CreateFamily([FromBody] FamilyModel family)
    {
        var created = _familyService.CreateFamily(family);

        if (!created.Success)
        {
            return BadRequest($"Error: {created.Error}");
        }

        return Ok("Created Family Successfully");
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
