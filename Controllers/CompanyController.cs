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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CompanyController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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
        public async Task<IActionResult> Create([Bind("Id,Name,Street,ExteriorNumber,InteriorNumber,Neighborhood,City,StateID,PostalCode,RFC,Email")] Company company, IFormFile? logo)
        {
            if (ModelState.IsValid)
            {
                // Handle logo upload
                if (logo != null && logo.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "logos");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(logo.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await logo.CopyToAsync(fileStream);
                    }

                    company.LogoPath = "/images/logos/" + uniqueFileName;
                }

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Street,ExteriorNumber,InteriorNumber,Neighborhood,City,StateID,PostalCode,RFC,Email,LogoPath")] Company company, IFormFile? logo)
        {
            if (ModelState.IsValid)
            {
                // Handle logo upload
                if (logo != null && logo.Length > 0)
                {
                    // Delete old logo if exists
                    if (!string.IsNullOrEmpty(company.LogoPath))
                    {
                        var oldLogoPath = Path.Combine(_webHostEnvironment.WebRootPath, company.LogoPath.TrimStart('/'));
                        if (System.IO.File.Exists(oldLogoPath))
                        {
                            System.IO.File.Delete(oldLogoPath);
                        }
                    }

                    // Upload new logo
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "logos");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(logo.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await logo.CopyToAsync(fileStream);
                    }

                    company.LogoPath = "/images/logos/" + uniqueFileName;
                }

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
