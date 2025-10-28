using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GrupoMad.Data;
using GrupoMad.Models;

namespace GrupoMad.Controllers
{
    public class ColorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ColorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Color
        public async Task<IActionResult> Index()
        {
            var colors = await _context.Colors
                .OrderBy(c => c.Name)
                .ToListAsync();
            return View(colors);
        }

        // GET: Color/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var color = await _context.Colors
                .Include(c => c.ProductColors)
                    .ThenInclude(pc => pc.Product)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (color == null)
            {
                return NotFound();
            }

            return View(color);
        }

        // GET: Color/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Color/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Code,Name,IsActive")] Color color)
        {
            if (ModelState.IsValid)
            {
                // Verificar que el código sea único
                var codeExists = await _context.Colors
                    .AnyAsync(c => c.Code == color.Code);

                if (codeExists)
                {
                    ModelState.AddModelError("Code", "Este código ya existe.");
                    return View(color);
                }

                // Verificar que el nombre sea único
                var nameExists = await _context.Colors
                    .AnyAsync(c => c.Name == color.Name);

                if (nameExists)
                {
                    ModelState.AddModelError("Name", "Este nombre ya existe.");
                    return View(color);
                }

                color.CreatedAt = DateTime.UtcNow;
                _context.Add(color);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Color creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(color);
        }

        // GET: Color/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var color = await _context.Colors.FindAsync(id);
            if (color == null)
            {
                return NotFound();
            }
            return View(color);
        }

        // POST: Color/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Name,IsActive")] Color color)
        {
            if (id != color.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar que el código sea único (excluyendo el color actual)
                    var codeExists = await _context.Colors
                        .AnyAsync(c => c.Code == color.Code && c.Id != id);

                    if (codeExists)
                    {
                        ModelState.AddModelError("Code", "Este código ya existe.");
                        return View(color);
                    }

                    // Verificar que el nombre sea único (excluyendo el color actual)
                    var nameExists = await _context.Colors
                        .AnyAsync(c => c.Name == color.Name && c.Id != id);

                    if (nameExists)
                    {
                        ModelState.AddModelError("Name", "Este nombre ya existe.");
                        return View(color);
                    }

                    var existingColor = await _context.Colors.FindAsync(id);
                    if (existingColor == null)
                    {
                        return NotFound();
                    }

                    existingColor.Code = color.Code;
                    existingColor.Name = color.Name;
                    existingColor.IsActive = color.IsActive;
                    existingColor.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Color actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ColorExists(color.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(color);
        }

        // GET: Color/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var color = await _context.Colors
                .Include(c => c.ProductColors)
                    .ThenInclude(pc => pc.Product)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (color == null)
            {
                return NotFound();
            }

            return View(color);
        }

        // POST: Color/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var color = await _context.Colors
                .Include(c => c.ProductColors)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (color == null)
            {
                return NotFound();
            }

            // Verificar si el color está siendo usado
            if (color.ProductColors != null && color.ProductColors.Any())
            {
                TempData["Error"] = $"No se puede eliminar el color '{color.Name}' porque está siendo usado por {color.ProductColors.Count} producto(s).";
                return RedirectToAction(nameof(Delete), new { id });
            }

            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Color eliminado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        private bool ColorExists(int id)
        {
            return _context.Colors.Any(e => e.Id == id);
        }
    }
}
