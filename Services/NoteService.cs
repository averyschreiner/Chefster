using Chefster.Common;
using Chefster.Context;
using Chefster.Interfaces;
using Chefster.Models;
using Microsoft.Data.SqlClient;

namespace Chefster.Services;

public class NoteService(MemberDbContext context) : INote
{
    private readonly MemberDbContext _context = context;

    public ServiceResult<WeeklyNotesModel> CreateNote(FamilyModel family)
    {
        throw new NotImplementedException();
    }

    public ServiceResult<WeeklyNotesModel> DeleteNote(string familyId, string noteId)
    {
        throw new NotImplementedException();
    }

    public ServiceResult<List<WeeklyNotesModel>> GetAll()
    {
        throw new NotImplementedException();
    }

    public ServiceResult<WeeklyNotesModel?> GetByFamilyId(string familyId)
    {
        throw new NotImplementedException();
    }

    public ServiceResult<WeeklyNotesModel> UpdateNote(string familyId, WeeklyNotesModel note)
    {
        throw new NotImplementedException();
    }
}