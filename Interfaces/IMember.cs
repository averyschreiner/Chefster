using Chefster.Models;
using Chefster.Common;

namespace Chefster.Interfaces;

public interface IMember
{
    ServiceResult<MemberModel?> GetByFamilyId(string id);
    ServiceResult<MemberModel> CreateMember(MemberModel member);
    ServiceResult<MemberModel> DeleteMember(string memberId, string familyId);
    ServiceResult<MemberModel> UpdateMember(MemberModel member);
}