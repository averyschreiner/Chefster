using System.ComponentModel.DataAnnotations.Schema;
using Chefster.Enums;
using Microsoft.EntityFrameworkCore;

namespace Chefster.Models;

[Table("WeeklyNotes")]
[PrimaryKey(nameof(ConsiderationId))]
public class ConsiderationsModel
{
    public required string ConsiderationId { get; set; }
    [ForeignKey(nameof(MemberId))]
    public required string MemberId { get; set; } // foreign key of family model
    public required ConsiderationsEnum Type { get; set; }
    public required string Value { get; set; }
    public required string CreatedAt { get; set; }
}

public class ConsiderationsDto
{
    public required ConsiderationsEnum Type { get; set; }
    public required string Value { get; set; }
}
