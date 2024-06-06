using Chefster.Common;
using Chefster.Context;
using Chefster.Interfaces;
using Chefster.Models;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace Chefster.Services;

public class NoteService(ChefsterDbContext context) : INote
{
    private readonly ChefsterDbContext _context = context;

    public ServiceResult<WeeklyNotesModel> CreateNote(string familyId, WeeklyNotesCreateDto note)
    {
        var n = new WeeklyNotesModel
        {
            NoteId = Guid.NewGuid().ToString("N"),
            FamilyId = familyId,
            Note = note.Note,
        };

        try
        {
            _context.WeeklyNotes.Add(n);
            _context.SaveChanges(); // Save changes to database after altering it
            return ServiceResult<WeeklyNotesModel>.SuccessResult(n);
        }
        catch (SqlException e)
        {
            return ServiceResult<WeeklyNotesModel>.ErrorResult(
                $"Failed to write note to database. Error: {e}"
            );
        }
    }

    public ServiceResult<WeeklyNotesModel> DeleteNote(string noteId)
    {
        try
        {
            var note = _context.WeeklyNotes.Find(noteId);
            if (note == null)
            {
                return ServiceResult<WeeklyNotesModel>.ErrorResult("Note doesn't exist");
            }
            _context.Remove(note);
            _context.SaveChanges();
            return ServiceResult<WeeklyNotesModel>.SuccessResult(note);
        }
        catch (Exception e)
        {
            return ServiceResult<WeeklyNotesModel>.ErrorResult(
                $"Failed to remove note from database. Error: {e}"
            );
        }
    }

    public ServiceResult<List<WeeklyNotesModel>> GetAllNotesFromFamily(string familyId)
    {
        try
        {
            var notes = _context.WeeklyNotes.Where(n => n.FamilyId == familyId).ToList();
            return ServiceResult<List<WeeklyNotesModel>>.SuccessResult(notes);
        }
        catch (SqlException e)
        {
            return ServiceResult<List<WeeklyNotesModel>>.ErrorResult(
                $"Failed to get all notes for family with Id {familyId}. Error: {e}"
            );
        }
    }

    public ServiceResult<WeeklyNotesModel> UpdateNote(string noteId, WeeklyNotesUpdateDto note)
    {
        try
        {
            var existingNote = _context.WeeklyNotes.Find(noteId);
            if (existingNote == null)
            {
                return ServiceResult<WeeklyNotesModel>.ErrorResult("note does not exist");
            }

            existingNote.Note = note.Note;
            _context.SaveChanges();
            return ServiceResult<WeeklyNotesModel>.SuccessResult(existingNote);
        }
        catch (SqlException e)
        {
            return ServiceResult<WeeklyNotesModel>.ErrorResult(
                $"Failed to update note with Id {noteId}. Error: {e}"
            );
        }
    }
}
