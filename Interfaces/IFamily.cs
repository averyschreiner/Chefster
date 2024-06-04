using Chefster.Models;
using Chefster.Common;

namespace Chefster.Interfaces;

public interface IFamily
{
    ServiceResult<List<FamilyModel>> GetAll();
    ServiceResult<FamilyModel?> GetById(string id);
    ServiceResult<FamilyModel> CreateFamily(FamilyModel family);
    ServiceResult<FamilyModel> DeleteFamily(string id);
    ServiceResult<FamilyModel> UpdateFamily(FamilyModel family);
}
