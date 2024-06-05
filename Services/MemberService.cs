using Chefster.Common;
using Chefster.Context;
using Chefster.Interfaces;
using Chefster.Models;
using Microsoft.Data.SqlClient;

namespace Chefster.Services;

public class MemberService(MemberDbContext context) : IMember
{
    private readonly MemberDbContext _context = context;

    public ServiceResult<MemberModel> CreateMember(MemberModel member)
    {
        throw new NotImplementedException();
    }

    public ServiceResult<MemberModel> DeleteMember(string familyId, string memberId)
    {
        throw new NotImplementedException();
    }

    public ServiceResult<MemberModel?> GetByFamilyId(string id)
    {
        throw new NotImplementedException();
    }

    public ServiceResult<MemberModel> UpdateMember(MemberModel member)
    {
        throw new NotImplementedException();
    }
}
