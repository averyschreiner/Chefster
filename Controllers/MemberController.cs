using Chefster.Models;
using Chefster.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Chefster.Controllers;

//[Authorize]
[Route("api/member")]
[ApiController]
public class MemberController(MemberService memberService) : ControllerBase
{
    private readonly MemberService _memberService = memberService;

    // slightly different route since we can use two different Ids to query members
    [HttpGet("/api/members/{FamilyId}")]
    public ActionResult<MemberModel> GetByFamilyId(string FamilyId)
    {
        var members = _memberService.GetByFamilyId(FamilyId);

        if (members.Data.IsNullOrEmpty())
        {
            return NotFound(new { Message = $"No members found under familyId: {FamilyId}" });
        }

        return Ok(members.Data);
    }

    [HttpGet("{MemberId}")]
    public ActionResult<MemberModel> GetByMemberId(string MemberId)
    {
        var members = _memberService.GetByMemberId(MemberId);

        if (members == null)
        {
            return NotFound(new { Message = $"No members found under familyId: {MemberId}" });
        }

        return Ok(members.Data);
    }

    [HttpPost("{FamilyId}")]
    public ActionResult CreateMember(string FamilyId, [FromBody] MemberCreateDto member)
    {
        var created = _memberService.CreateMember(FamilyId, member);

        if (!created.Success)
        {
            return BadRequest($"Error: {created.Error}");
        }

        return Ok("Created member Successfully");
    }

    [HttpDelete("{Id}")]
    public ActionResult DeleteMember(string Id)
    {
        var deleted = _memberService.DeleteMember(Id);

        if (!deleted.Success)
        {
            return BadRequest($"Error: {deleted.Error}");
        }

        return Ok();
    }

    [HttpPut("{MemberId}")]
    public ActionResult<MemberModel> UpdateMember([FromBody] MemberUpdateDto member, string MemberId)
    {
        var updated = _memberService.UpdateMember(MemberId, member);

        if (!updated.Success)
        {
            return BadRequest($"Error: {updated.Error}");
        }

        return Ok(updated.Data);
    }
}
