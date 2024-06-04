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
    public ActionResult<FamilyModel> GetFamily(string id)
    {
        var family = _familyService.GetById(id);

        if (family == null)
        {
            return NotFound(new { Message = $"No family found with familyId {id}" });
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
    public ActionResult<FamilyModel> CreateFamily([FromBody] FamilyModel family)
    {
        var created = _familyService.CreateFamily(family);

        if (!created.Success)
        {
            return BadRequest($"Error: {created.Error}");
        }

        return Ok(created.Data);
    }

    [HttpDelete("{Id}")]
    public ActionResult DeleteFamily(string id)
    {
        var deleted = _familyService.DeleteFamily(id);

        if (!deleted.Success)
        {
            return BadRequest($"Error: {deleted.Error}");
        }

        return Ok();
    }

    [HttpPut]
    public ActionResult<FamilyModel> UpdateFamily([FromBody] FamilyModel family)
    {
        var updated = _familyService.UpdateFamily(family);

        if (!updated.Success)
        {
            return BadRequest($"Error: {updated.Error}");
        }

        return Ok(updated.Data);
    }
}
