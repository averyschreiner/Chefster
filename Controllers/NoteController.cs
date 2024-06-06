using Chefster.Models;
using Chefster.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chefster.Controllers;

[Authorize]
[Route("api/note")]
[ApiController]
public class NoteController(NoteService noteService) : ControllerBase
{
    private readonly NoteService _noteService = noteService;

    [HttpGet("{FamilyId}")]
    public ActionResult<WeeklyNotesModel> GetByFamilyId(string FamilyId)
    {
        var note = _noteService.GetAllNotesFromFamily(FamilyId);
        if (note == null)
        {
            return BadRequest($"not notes for family with Id {FamilyId}");
        }

        return Ok(note.Data);
    }

    /// <summary>
    /// Gets Family Notes from the last 7 days
    /// </summary>
    [HttpGet("/api/notes/{FamilyId}")]
    public ActionResult<WeeklyNotesModel> GetPreviousWeekNotes(string FamilyId)
    {
        var note = _noteService.GetWeeklyNotes(FamilyId);
        if (note == null)
        {
            return BadRequest($"not notes for family with Id {FamilyId} for the last 7 days");
        }

        return Ok(note.Data);
    }

    [HttpPost("{FamilyId}")]
    public ActionResult CreateNote(string FamilyId, [FromBody] WeeklyNotesCreateDto note)
    {
        var created = _noteService.CreateNote(FamilyId, note);
        if (!created.Success)
        {
            return BadRequest($"Error: {created.Error}");
        }

        return Ok("Created note successfully");
    }

    [HttpDelete("{NoteId}")]
    public ActionResult DeleteNote(string NoteId)
    {
        var deleted = _noteService.DeleteNote(NoteId);
        if (!deleted.Success)
        {
            return BadRequest($"Error: {deleted.Error}");
        }

        return Ok("Successfully deleted note");
    }

    [HttpPut("{NoteId}")]
    public ActionResult<WeeklyNotesModel> UpdateNote(
        string NoteId,
        [FromBody] WeeklyNotesUpdateDto note
    )
    {
        var updated = _noteService.UpdateNote(NoteId, note);

        if (!updated.Success)
        {
            return BadRequest($"Error: {updated.Error}");
        }

        return Ok(updated.Data);
    }
}
