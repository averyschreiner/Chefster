namespace Chefster.Models;

public class MemberModel
{
    public required string name;
    //This is the Family that the member is a child of
    public required string FamilyId;
    public string[]? foodAllergies;
    public string[]? dietaryRestrictions;
    public string[]? dietGoals;
    public string[]? preferences;
}