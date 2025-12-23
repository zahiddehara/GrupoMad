using GrupoMad.Data;
using GrupoMad.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace GrupoMad.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompanyController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var companies = await _context.Companies.ToListAsync();
            return View(companies);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Name,Street,ExteriorNumber,InteriorNumber,Neighborhood,City,StateID,PostalCode,RFC,Email")] Company company)
        {
            if (ModelState.IsValid)
            {
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(company);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == id);
            return View(company);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Street,ExteriorNumber,InteriorNumber,Neighborhood,City,StateID,PostalCode,RFC,Email")] Company company)
        {
            if (ModelState.IsValid)
            {
                _context.Update(company);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(company);
        }

        public async Task<IActionResult> Details(int id)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }
    }
}
