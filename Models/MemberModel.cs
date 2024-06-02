namespace Chefster.Models;

public class MemberModel
{
    public required string name;
    //This is the User that the member is a child of
    public required string UserId;
    public string[]? foodAllergies;
    public string[]? dietaryRestrictions;
    public string[]? dietGoals;
    public string[]? preferences;
}