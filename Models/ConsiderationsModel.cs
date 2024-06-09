using System.ComponentModel.DataAnnotations.Schema;
using Chefster.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

public class ConsiderationsCreateDto
{
    public required string MemberId { get; set; }
    public required ConsiderationsEnum Type { get; set; }
    public required string Value { get; set; }
}

public class ConsiderationsUpdateDto
{
    public required string Id { get; set; }
    public required ConsiderationsEnum Type { get; set; }
    public required string Value { get; set; }
}
