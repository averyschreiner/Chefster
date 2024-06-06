using Chefster.Models;
using Chefster.Common;

namespace Chefster.Interfaces;

public interface INote
{
    ServiceResult<List<WeeklyNotesModel>> GetAllNotesFromFamily(string familyId);
    ServiceResult<WeeklyNotesModel> CreateNote(string familyId, WeeklyNotesCreateDto note);
    ServiceResult<WeeklyNotesModel> DeleteNote(string noteId);
    ServiceResult<WeeklyNotesModel> UpdateNote(string noteId, WeeklyNotesUpdateDto note);
}
