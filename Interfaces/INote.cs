using Chefster.Models;
using Chefster.Common;

namespace Chefster.Interfaces;

public interface INote
{
    ServiceResult<List<WeeklyNotesModel>> GetAll();
    ServiceResult<WeeklyNotesModel?> GetByFamilyId(string familyId);
    ServiceResult<WeeklyNotesModel> CreateNote(FamilyModel family);
    ServiceResult<WeeklyNotesModel> DeleteNote(string familyId, string noteId);
    ServiceResult<WeeklyNotesModel> UpdateNote(string familyId, WeeklyNotesModel note);
}
