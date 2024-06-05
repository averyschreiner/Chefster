using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Chefster.Models;

[Table("WeeklyNotes")]
[PrimaryKey(nameof(FamilyId))]
public class WeeklyNotesModel
{
    public required string FamilyId { get; set; }
    public required string Note { get; set; }
}
