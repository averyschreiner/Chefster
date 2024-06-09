using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Chefster.Models;

[Table("Members")]
[PrimaryKey(nameof(MemberId))]
public class MemberModel
{
    public required string MemberId { get; set; }
    //This is the Family that the member is a child of
    public required string FamilyId { get; set; }
    public required string Name { get; set; }
}

public class MemberUpdateDto
{
    public required string Name { get; set; }
}

public class MemberCreateDto
{
    public required string FamilyId { get; set; }
    public required string Name { get; set; }
}
