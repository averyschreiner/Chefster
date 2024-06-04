namespace Chefster.Models;

public class FamilyModel
{
    public required string id;
    public string? email;
    public string? phoneNumber;
    public required string createdAt;
    public required int familySize;
    public MemberModel[]? members;
    public string[]? weeklyNotes;
}