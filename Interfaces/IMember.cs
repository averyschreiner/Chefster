using Chefster.Models;
using Chefster.Common;

namespace Chefster.Interfaces;

public interface IMember
{
    ServiceResult<List<MemberModel>> GetByFamilyId(string id);
    ServiceResult<MemberModel> CreateMember(string familyId, MemberCreateDto member);
    ServiceResult<MemberModel> DeleteMember(string memberId);
    ServiceResult<MemberModel> UpdateMember(string memberId, MemberUpdateDto member);
}