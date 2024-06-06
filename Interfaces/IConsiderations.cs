using Chefster.Models;
using Chefster.Common;

namespace Chefster.Interfaces;

public interface IConsiderations
{
    ServiceResult<List<ConsiderationsModel>> GetAllFamilyConsiderations(string familyId);
    ServiceResult<List<ConsiderationsModel>> GetWeeklyConsiderations(string familyId);
    ServiceResult<ConsiderationsModel> CreateConsideration(string familyId, ConsiderationsDto note);
    ServiceResult<ConsiderationsModel> DeleteConsideration(string noteId);
    ServiceResult<ConsiderationsModel> UpdateConsideration(string noteId, ConsiderationsDto note);
}
