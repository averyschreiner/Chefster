using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System;
using Swashbuckle.AspNetCore.Annotations;

namespace Chefster.Models;

[Table("Families")]
[PrimaryKey(nameof(Id))]
public class FamilyModel
{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string CreatedAt { get; set; }
    public required int FamilySize { get; set; }
    public required string GenerationDay { get; set; }
    public required string GenerationTime { get; set; }

    [NotMapped] // these are part of the model but not part of the table in the db
    [SwaggerIgnore]
    public List<MemberModel?>? Members { get; set; }

    [NotMapped]
    [SwaggerIgnore]
    public List<ConsiderationsModel?>? Considerations { get; set; }
}

/*
This class is for when we update a Family Object
dto stands for data transfer object. We use this object to show only the values
that we are ok with editing
*/
public class FamilyUpdateDto
{
    public required string PhoneNumber { get; set; }
    public required int FamilySize { get; set; }
    public required string GenerationDay { get; set; }
    public required string GenerationTime { get; set; }
}
