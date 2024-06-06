using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Chefster.Models;

[Table("WeeklyNotes")]
[PrimaryKey(nameof(NoteId))]
public class WeeklyNotesModel
{
    public required string NoteId { get; set; }
    public required string FamilyId { get; set; }
    public required string Note { get; set; }
}

public class WeeklyNotesUpdateDto
{
    public required string Note { get; set; }
}

public class WeeklyNotesCreateDto
{
    public required string Note { get; set; }
}
