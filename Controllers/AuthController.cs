using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using GrupoMad.Data;
using GrupoMad.Models;

namespace GrupoMad.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Auth/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            // Si ya está autenticado, redirigir a Home
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "El email y la contraseña son requeridos.");
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }

            // Buscar usuario por email
            var user = await _context.Users
                .Include(u => u.Store)
                    .ThenInclude(s => s.Company)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }

            // Verificar que el usuario esté activo
            if (!user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Esta cuenta ha sido desactivada. Contacta al administrador.");
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }

            // Verificar contraseña
            bool passwordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!passwordValid)
            {
                ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }

            // Actualizar LastLoginAt
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Crear claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName)
            };

            // Agregar StoreId si existe
            if (user.StoreId.HasValue)
            {
                claims.Add(new Claim("StoreId", user.StoreId.Value.ToString()));
                claims.Add(new Claim("StoreName", user.Store?.Name ?? ""));
                claims.Add(new Claim("CompanyId", user.Store?.CompanyId?.ToString() ?? ""));
                claims.Add(new Claim("CompanyName", user.Store?.Company?.Name ?? ""));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // "Recordarme"
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Redirigir
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Auth/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // GET: Auth/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
