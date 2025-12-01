using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using GrupoMad.Data;
using GrupoMad.Models;
using System.Security.Claims;

namespace GrupoMad.Controllers
{
    [Authorize]
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return User.IsInRole(UserRole.Administrator.ToString());
        }

        private int? GetUserStoreId()
        {
            var storeIdClaim = User.FindFirst("StoreId")?.Value;
            return int.TryParse(storeIdClaim, out int storeId) ? storeId : null;
        }

        private async Task<int?> GetUserCompanyIdAsync()
        {
            var storeId = GetUserStoreId();
            if (!storeId.HasValue)
                return null;

            var store = await _context.Stores.FindAsync(storeId.Value);
            return store?.CompanyId;
        }

        // GET: Contact
        public async Task<IActionResult> Index(string searchTerm, int? companyId)
        {
            var isAdmin = IsAdmin();

            // Si no es admin, forzar el filtro por la empresa del usuario
            if (!isAdmin)
            {
                companyId = await GetUserCompanyIdAsync();
            }

            var query = _context.Contacts
                .Include(c => c.Company)
                .Include(c => c.ShippingAddresses)
                .AsQueryable();

            // Filtrar por término de búsqueda
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(c =>
                    c.FirstName.ToLower().Contains(searchTerm) ||
                    (c.LastName != null && c.LastName.ToLower().Contains(searchTerm)) ||
                    (c.RFC != null && c.RFC.ToLower().Contains(searchTerm)) ||
                    (c.Email != null && c.Email.ToLower().Contains(searchTerm)) ||
                    (c.City != null && c.City.ToLower().Contains(searchTerm)) ||
                    c.Company.Name.ToLower().Contains(searchTerm)
                );
            }

            // Filtrar por empresa
            if (companyId.HasValue)
            {
                query = query.Where(c => c.CompanyId == companyId.Value);
            }

            var contacts = await query
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName)
                .ToListAsync();

            // Pasar datos para filtros
            ViewBag.SearchTerm = searchTerm;
            ViewBag.CompanyId = companyId;
            ViewBag.IsAdmin = isAdmin;

            // Solo cargar empresas si es admin
            if (isAdmin)
            {
                ViewBag.Companies = new SelectList(await _context.Companies.OrderBy(c => c.Name).ToListAsync(), "Id", "Name", companyId);
            }

            return View(contacts);
        }

        // GET: Contact/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.Company)
                .Include(c => c.ShippingAddresses)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // GET: Contact/Create
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name");
            return View();
        }

        // POST: Contact/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Street,ExteriorNumber,InteriorNumber,Neighborhood,City,StateID,PostalCode,RFC,Email,CompanyId")] Contact contact)
        {
            ModelState.Remove("Company");
            ModelState.Remove("ShippingAddresses");

            if (ModelState.IsValid)
            {
                contact.CreatedAt = DateTime.UtcNow;
                _context.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", contact.CompanyId);
            return View(contact);
        }

        // GET: Contact/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", contact.CompanyId);
            return View(contact);
        }

        // POST: Contact/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Street,ExteriorNumber,InteriorNumber,Neighborhood,City,StateID,PostalCode,RFC,Email,CompanyId,CreatedAt")] Contact contact)
        {
            if (id != contact.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Company");
            ModelState.Remove("ShippingAddresses");

            if (ModelState.IsValid)
            {
                try
                {
                    contact.UpdatedAt = DateTime.UtcNow;
                    _context.Update(contact);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.Id))
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
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", contact.CompanyId);
            return View(contact);
        }

        // GET: Contact/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.Company)
                .Include(c => c.ShippingAddresses)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: Contact/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.Contacts
                .Include(c => c.ShippingAddresses)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contact != null)
            {
                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
            return _context.Contacts.Any(e => e.Id == id);
        }

        // ==================== SHIPPING ADDRESS MANAGEMENT ====================

        // GET: Contact/CreateShippingAddress/5
        public async Task<IActionResult> CreateShippingAddress(int? contactId)
        {
            if (contactId == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts.FindAsync(contactId);
            if (contact == null)
            {
                return NotFound();
            }

            ViewBag.Contact = contact;
            var shippingAddress = new ShippingAddress
            {
                ContactID = contact.Id,
                FirstName = contact.FirstName,
                LastName = contact.LastName ?? string.Empty,
                Street = contact.Street ?? string.Empty,
                ExteriorNumber = contact.ExteriorNumber ?? string.Empty,
                InteriorNumber = contact.InteriorNumber,
                Neighborhood = contact.Neighborhood ?? string.Empty,
                City = contact.City ?? string.Empty,
                StateID = contact.StateID ?? MexicanState.Aguascalientes,
                PostalCode = contact.PostalCode ?? string.Empty,
                RFC = contact.RFC
            };

            return View(shippingAddress);
        }

        // POST: Contact/CreateShippingAddress
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateShippingAddress([Bind("Id,FirstName,LastName,Street,ExteriorNumber,InteriorNumber,Neighborhood,City,StateID,PostalCode,RFC,ContactID,DeliveryReference")] ShippingAddress shippingAddress)
        {
            ModelState.Remove("Contact");

            if (ModelState.IsValid)
            {
                shippingAddress.CreatedAt = DateTime.UtcNow;
                _context.Add(shippingAddress);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = shippingAddress.ContactID });
            }

            var contact = await _context.Contacts.FindAsync(shippingAddress.ContactID);
            ViewBag.Contact = contact;
            return View(shippingAddress);
        }

        // GET: Contact/EditShippingAddress/5
        public async Task<IActionResult> EditShippingAddress(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shippingAddress = await _context.ShippingAddresses
                .Include(sa => sa.Contact)
                .FirstOrDefaultAsync(sa => sa.Id == id);

            if (shippingAddress == null)
            {
                return NotFound();
            }

            ViewBag.Contact = shippingAddress.Contact;
            return View(shippingAddress);
        }

        // POST: Contact/EditShippingAddress/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditShippingAddress(int id, [Bind("Id,FirstName,LastName,Street,ExteriorNumber,InteriorNumber,Neighborhood,City,StateID,PostalCode,RFC,ContactID,DeliveryReference,CreatedAt")] ShippingAddress shippingAddress)
        {
            if (id != shippingAddress.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Contact");

            if (ModelState.IsValid)
            {
                try
                {
                    shippingAddress.UpdatedAt = DateTime.UtcNow;
                    _context.Update(shippingAddress);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShippingAddressExists(shippingAddress.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = shippingAddress.ContactID });
            }

            var contact = await _context.Contacts.FindAsync(shippingAddress.ContactID);
            ViewBag.Contact = contact;
            return View(shippingAddress);
        }

        // GET: Contact/DeleteShippingAddress/5
        public async Task<IActionResult> DeleteShippingAddress(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shippingAddress = await _context.ShippingAddresses
                .Include(sa => sa.Contact)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (shippingAddress == null)
            {
                return NotFound();
            }

            return View(shippingAddress);
        }

        // POST: Contact/DeleteShippingAddress/5
        [HttpPost, ActionName("DeleteShippingAddress")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteShippingAddressConfirmed(int id)
        {
            var shippingAddress = await _context.ShippingAddresses.FindAsync(id);
            int contactId = shippingAddress?.ContactID ?? 0;

            if (shippingAddress != null)
            {
                _context.ShippingAddresses.Remove(shippingAddress);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = contactId });
        }

        private bool ShippingAddressExists(int id)
        {
            return _context.ShippingAddresses.Any(e => e.Id == id);
        }
    }
}
