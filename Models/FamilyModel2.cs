using System;

namespace Chefster.Models;

public class FamilyModel2
{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber{ get; set; }
    public required int FamilySize { get; set; }
    public required DayOfWeek GenerationDay { get; set; }
    public required TimeSpan GenerationTime { get; set; }
}
