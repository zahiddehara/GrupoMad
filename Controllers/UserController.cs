using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using GrupoMad.Data;
using GrupoMad.Models;
using BCrypt.Net;

namespace GrupoMad.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .Include(u => u.Store)
                    .ThenInclude(s => s.Company)
                .ToListAsync();
            return View(users);
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Store)
                    .ThenInclude(s => s.Company)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            ViewData["StoreId"] = new SelectList(_context.Stores.Include(s => s.Company), "Id", "Name");
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,PhoneNumber,Role,StoreId,IsActive")] User user, string Password)
        {
            ModelState.Remove("PasswordHash");
            ModelState.Remove("Store");

            // Validación: Email único
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Este email ya está registrado.");
            }

            // Validación: Contraseña requerida
            if (string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError("Password", "La contraseña es requerida.");
            }
            // Validación: Contraseña fuerte
            else if (Password.Length < 8)
            {
                ModelState.AddModelError("Password", "La contraseña debe tener al menos 8 caracteres.");
            }
            else if (!Password.Any(char.IsUpper))
            {
                ModelState.AddModelError("Password", "La contraseña debe contener al menos una letra mayúscula.");
            }
            else if (!Password.Any(char.IsLower))
            {
                ModelState.AddModelError("Password", "La contraseña debe contener al menos una letra minúscula.");
            }
            else if (!Password.Any(char.IsDigit))
            {
                ModelState.AddModelError("Password", "La contraseña debe contener al menos un número.");
            }

            // Validación: StoreId requerido para no-administradores
            if (user.Role != UserRole.Administrator && user.StoreId == null)
            {
                ModelState.AddModelError("StoreId", "Los usuarios que no son administradores deben tener una tienda asignada.");
            }

            // Validación: Administrator NO debe tener StoreId
            if (user.Role == UserRole.Administrator && user.StoreId != null)
            {
                ModelState.AddModelError("StoreId", "Los administradores no deben tener una tienda específica asignada.");
            }

            if (ModelState.IsValid)
            {
                // Hashear la contraseña
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password);
                user.CreatedAt = DateTime.UtcNow;

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["StoreId"] = new SelectList(_context.Stores.Include(s => s.Company), "Id", "Name", user.StoreId);
            return View(user);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            ViewData["StoreId"] = new SelectList(_context.Stores.Include(s => s.Company), "Id", "Name", user.StoreId);
            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,PhoneNumber,Role,StoreId,IsActive,CreatedAt")] User user, string Password)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            ModelState.Remove("PasswordHash");
            ModelState.Remove("Store");

            // Validación: Email único (excluyendo el usuario actual)
            if (await _context.Users.AnyAsync(u => u.Email == user.Email && u.Id != user.Id))
            {
                ModelState.AddModelError("Email", "Este email ya está registrado por otro usuario.");
            }

            // Validación de contraseña solo si se proporciona una nueva
            if (!string.IsNullOrWhiteSpace(Password))
            {
                if (Password.Length < 8)
                {
                    ModelState.AddModelError("Password", "La contraseña debe tener al menos 8 caracteres.");
                }
                else if (!Password.Any(char.IsUpper))
                {
                    ModelState.AddModelError("Password", "La contraseña debe contener al menos una letra mayúscula.");
                }
                else if (!Password.Any(char.IsLower))
                {
                    ModelState.AddModelError("Password", "La contraseña debe contener al menos una letra minúscula.");
                }
                else if (!Password.Any(char.IsDigit))
                {
                    ModelState.AddModelError("Password", "La contraseña debe contener al menos un número.");
                }
            }

            // Validación: StoreId requerido para no-administradores
            if (user.Role != UserRole.Administrator && user.StoreId == null)
            {
                ModelState.AddModelError("StoreId", "Los usuarios que no son administradores deben tener una tienda asignada.");
            }

            // Validación: Administrator NO debe tener StoreId
            if (user.Role == UserRole.Administrator && user.StoreId != null)
            {
                ModelState.AddModelError("StoreId", "Los administradores no deben tener una tienda específica asignada.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

                    // Solo actualizar contraseña si se proporcionó una nueva
                    if (!string.IsNullOrWhiteSpace(Password))
                    {
                        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password);
                    }
                    else
                    {
                        user.PasswordHash = existingUser.PasswordHash;
                    }

                    user.UpdatedAt = DateTime.UtcNow;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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

            ViewData["StoreId"] = new SelectList(_context.Stores.Include(s => s.Company), "Id", "Name", user.StoreId);
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Store)
                    .ThenInclude(s => s.Company)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: User/ChangePassword/5
        public async Task<IActionResult> ChangePassword(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/ChangePassword/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(int id, string NewPassword, string ConfirmPassword)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                ModelState.AddModelError("NewPassword", "La contraseña es requerida.");
            }
            else if (NewPassword.Length < 8)
            {
                ModelState.AddModelError("NewPassword", "La contraseña debe tener al menos 8 caracteres.");
            }
            else if (!NewPassword.Any(char.IsUpper))
            {
                ModelState.AddModelError("NewPassword", "La contraseña debe contener al menos una letra mayúscula.");
            }
            else if (!NewPassword.Any(char.IsLower))
            {
                ModelState.AddModelError("NewPassword", "La contraseña debe contener al menos una letra minúscula.");
            }
            else if (!NewPassword.Any(char.IsDigit))
            {
                ModelState.AddModelError("NewPassword", "La contraseña debe contener al menos un número.");
            }

            if (NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Las contraseñas no coinciden.");
            }

            if (ModelState.IsValid)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(NewPassword);
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Contraseña actualizada exitosamente.";
                return RedirectToAction(nameof(Details), new { id = user.Id });
            }

            return View(user);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
