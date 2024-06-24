using Microsoft.AspNetCore.Mvc.Rendering;

namespace Chefster.ViewModels;

public class MemberUpdateViewModel
{
    public required string MemberId { get; set; }
    public required string Name { get; set; }
    public string? Notes { get; set; }
    public required List<SelectListItem> Restrictions { get; set; }
    public required List<SelectListItem> Goals { get; set; }
    public required List<SelectListItem> Cuisines { get; set; }
    public int Index { get; set; }
}