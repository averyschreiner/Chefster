using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Chefster.Models;

[Table("Families")]
[PrimaryKey(nameof(Id))]
public class FamilyModel
{
    public required string Id { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber{ get; set; }
    public required string CreatedAt { get; set; }
    public required int FamilySize { get; set; }
    [NotMapped] // these are part of the model but not part of the table in the db
    [SwaggerIgnore]
    public MemberModel[]? Members { get; set; }
    [NotMapped]
    [SwaggerIgnore]
    public string[]? WeeklyNotes { get; set; }
}