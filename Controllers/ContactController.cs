using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using GrupoMad.Data;
using GrupoMad.Models;

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

        // GET: Contact
        public async Task<IActionResult> Index()
        {
            var contacts = await _context.Contacts
                .Include(c => c.Company)
                .Include(c => c.ShippingAddresses)
                .ToListAsync();
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
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Street,ExteriorNumber,InteriorNumber,Neighborhood,City,StateID,PostalCode,RFC,CompanyId")] Contact contact)
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Street,ExteriorNumber,InteriorNumber,Neighborhood,City,StateID,PostalCode,RFC,CompanyId,CreatedAt")] Contact contact)
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
                LastName = contact.LastName,
                Street = contact.Street,
                ExteriorNumber = contact.ExteriorNumber,
                InteriorNumber = contact.InteriorNumber,
                Neighborhood = contact.Neighborhood,
                City = contact.City,
                StateID = contact.StateID,
                PostalCode = contact.PostalCode,
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
