using Microsoft.AspNetCore.Mvc;
using GrupoMad.Models.ViewComponents;

namespace GrupoMad.ViewComponents
{
    public class DataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(object config)
        {
            return View("Default", config);
        }
    }
}
