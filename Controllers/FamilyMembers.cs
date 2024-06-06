using Microsoft.AspNetCore.Mvc;

namespace Chefster.ViewComponents
{
    public class FamilyMembersFormViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(int index)
        {
            return View(index);
        }
    }
}
