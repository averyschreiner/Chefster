using Chefster.Models;

namespace Chefster.Models;

public class UserModel
{
    public required string id;
    public string? email;
    public string? phoneNumber;
    public required string createdAt;
    public required int familySize;
    public MemberModel[]? members;
    public string[]? weeklyNotes;
}