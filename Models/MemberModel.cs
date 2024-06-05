using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Chefster.Models;

[Table("Members")]
[PrimaryKey(nameof(FamilyId))]
public class MemberModel
{
    public required string MemberId { get; set; }

    //This is the Family that the member is a child of
    public required string FamilyId { get; set; }
    public required string Name { get; set; }
    public string? DietaryRestrictions { get; set; }
    public string? DietGoals { get; set; }
    public string? Preferences { get; set; }
}
