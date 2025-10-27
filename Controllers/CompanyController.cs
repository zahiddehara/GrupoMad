using GrupoMad.Models;
using Microsoft.AspNetCore.Mvc;

namespace GrupoMad.Controllers
{
    public class CompanyController : Controller
    {
        public IActionResult Index()
        {
            var company = new Company() { Name = "Deconolux" };
            company.Name = "Persitex";
            return View(company);
        }
    }
}
