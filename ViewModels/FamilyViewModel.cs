namespace Chefster.ViewModels;

public class FamilyViewModel
{
    public required string PhoneNumber { get; set; }
    public required int FamilySize { get; set; }
    public required DayOfWeek GenerationDay { get; set; }
    public required TimeSpan GenerationTime { get; set; }
    public required List<MemberViewModel> Members { get; set; }
}