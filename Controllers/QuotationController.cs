using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using GrupoMad.Data;
using GrupoMad.Models;
using GrupoMad.Services;
using System.Security.Claims;

namespace GrupoMad.Controllers
{
    [Authorize]
    public class QuotationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly QuotationService _quotationService;

        public QuotationController(ApplicationDbContext context, QuotationService quotationService)
        {
            _context = context;
            _quotationService = quotationService;
        }

        // GET: Quotation
        public async Task<IActionResult> Index(QuotationStatus? status, int? contactId)
        {
            var query = _context.Quotations
                .Include(q => q.Contact)
                .Include(q => q.Store)
                .Include(q => q.CreatedByUser)
                .Include(q => q.Items)
                .AsQueryable();

            // Filtrar por tienda del usuario (si no es admin)
            var userStoreId = GetUserStoreId();
            if (userStoreId.HasValue)
            {
                query = query.Where(q => q.StoreId == userStoreId.Value);
            }

            // Filtrar por estado
            if (status.HasValue)
            {
                query = query.Where(q => q.Status == status.Value);
            }

            // Filtrar por contacto
            if (contactId.HasValue)
            {
                query = query.Where(q => q.ContactId == contactId.Value);
            }

            var quotations = await query
                .OrderByDescending(q => q.QuotationDate)
                .ToListAsync();

            ViewBag.StatusFilter = status;
            ViewBag.ContactIdFilter = contactId;

            // Cargar lista de contactos para el filtro
            var contactsQuery = _context.Contacts.AsQueryable();
            if (userStoreId.HasValue)
            {
                var store = _context.Stores.Find(userStoreId.Value);
                if (store?.CompanyId != null)
                {
                    contactsQuery = contactsQuery.Where(c => c.CompanyId == store.CompanyId);
                }
            }
            var contacts = await contactsQuery
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName)
                .Select(c => new {
                    Id = c.Id,
                    FullName = c.FirstName + " " + c.LastName
                })
                .ToListAsync();
            ViewBag.Contacts = new SelectList(contacts, "Id", "FullName", contactId);

            return View(quotations);
        }

        // GET: Quotation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quotation = await _context.Quotations
                .Include(q => q.Contact)
                .Include(q => q.Store)
                    .ThenInclude(s => s.Company)
                .Include(q => q.CreatedByUser)
                .Include(q => q.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.ProductType)
                .Include(q => q.Items)
                    .ThenInclude(i => i.ProductColor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (quotation == null)
            {
                return NotFound();
            }

            // Verificar acceso por tienda
            if (!CanAccessQuotation(quotation))
            {
                return Forbid();
            }

            return View(quotation);
        }

        // GET: Quotation/Create
        public IActionResult Create(int? contactId)
        {
            var userStoreId = GetUserStoreId();

            // Si no es admin y no tiene tienda, error
            if (!userStoreId.HasValue && !IsAdmin())
            {
                return BadRequest("Usuario sin tienda asignada");
            }

            // Preparar ViewData
            PrepareViewData(userStoreId, contactId);

            var quotation = new Quotation
            {
                QuotationDate = DateTime.UtcNow,
                ValidUntil = _quotationService.GetDefaultValidUntil(),
                StoreId = userStoreId ?? 0
            };

            // Si se especificó un contacto, pre-cargar su información
            if (contactId.HasValue)
            {
                var contact = _context.Contacts
                    .Include(c => c.ShippingAddresses)
                    .FirstOrDefault(c => c.Id == contactId.Value);

                if (contact != null)
                {
                    quotation.ContactId = contact.Id;
                    // Pre-cargar con la dirección principal del contacto
                    quotation.DeliveryFirstName = contact.FirstName;
                    quotation.DeliveryLastName = contact.LastName ?? string.Empty;
                    quotation.DeliveryStreet = contact.Street ?? string.Empty;
                    quotation.DeliveryExteriorNumber = contact.ExteriorNumber ?? string.Empty;
                    quotation.DeliveryInteriorNumber = contact.InteriorNumber;
                    quotation.DeliveryNeighborhood = contact.Neighborhood ?? string.Empty;
                    quotation.DeliveryCity = contact.City ?? string.Empty;
                    quotation.DeliveryStateID = contact.StateID ?? MexicanState.Aguascalientes;
                    quotation.DeliveryPostalCode = contact.PostalCode ?? string.Empty;
                    quotation.DeliveryRFC = contact.RFC;
                }
            }

            return View(quotation);
        }

        // POST: Quotation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Quotation quotation, List<QuotationItemDto> items)
        {
            ModelState.Remove("Contact");
            ModelState.Remove("Store");
            ModelState.Remove("CreatedByUser");
            ModelState.Remove("Items");
            ModelState.Remove("QuotationNumber");

            // Remover validaciones de propiedades de navegación en items
            if (items != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    ModelState.Remove($"items[{i}].Product");
                    ModelState.Remove($"items[{i}].ProductColor");
                    ModelState.Remove($"items[{i}].Quotation");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Generar número de cotización
                    quotation.QuotationNumber = await _quotationService.GenerateQuotationNumberAsync(quotation.StoreId);

                    // Asignar usuario que crea
                    quotation.CreatedByUserId = GetUserId();
                    quotation.CreatedAt = DateTime.UtcNow;
                    quotation.Status = QuotationStatus.Draft;

                    // Procesar items
                    if (items != null && items.Any())
                    {
                        quotation.Items = new List<QuotationItem>();
                        for (int i = 0; i < items.Count; i++)
                        {
                            var itemDto = items[i];
                            var quotationItem = new QuotationItem
                            {
                                ProductId = itemDto.ProductId,
                                ProductColorId = itemDto.ProductColorId,
                                Variant = itemDto.Variant,
                                Quantity = itemDto.Quantity,
                                UnitPrice = itemDto.UnitPrice,
                                DiscountedPrice = itemDto.DiscountedPrice,
                                Width = itemDto.Width,
                                Height = itemDto.Height,
                                Description = itemDto.Description,
                                DisplayOrder = i
                            };
                            quotation.Items.Add(quotationItem);
                        }
                    }

                    _context.Add(quotation);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cotización creada exitosamente";
                    return RedirectToAction(nameof(Details), new { id = quotation.Id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al crear la cotización: {ex.Message}");
                }
            }

            var userStoreId = GetUserStoreId();
            PrepareViewData(userStoreId, quotation.ContactId);
            return View(quotation);
        }

        // GET: Quotation/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quotation = await _context.Quotations
                .Include(q => q.Items)
                    .ThenInclude(i => i.Product)
                .Include(q => q.Items)
                    .ThenInclude(i => i.ProductColor)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quotation == null)
            {
                return NotFound();
            }

            if (!CanAccessQuotation(quotation))
            {
                return Forbid();
            }

            if (!quotation.CanBeEdited())
            {
                TempData["ErrorMessage"] = "Solo se pueden editar cotizaciones en estado Borrador";
                return RedirectToAction(nameof(Details), new { id = quotation.Id });
            }

            var userStoreId = GetUserStoreId();
            PrepareViewData(userStoreId, quotation.ContactId);

            return View(quotation);
        }

        // POST: Quotation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Quotation quotation, List<QuotationItemDto> items)
        {
            if (id != quotation.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Contact");
            ModelState.Remove("Store");
            ModelState.Remove("CreatedByUser");
            ModelState.Remove("Items");
            ModelState.Remove("QuotationNumber");

            // Remover validaciones de propiedades de navegación en items
            if (items != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    ModelState.Remove($"items[{i}].Product");
                    ModelState.Remove($"items[{i}].ProductColor");
                    ModelState.Remove($"items[{i}].Quotation");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingQuotation = await _context.Quotations
                        .Include(q => q.Items)
                        .FirstOrDefaultAsync(q => q.Id == id);

                    if (existingQuotation == null)
                    {
                        return NotFound();
                    }

                    if (!existingQuotation.CanBeEdited())
                    {
                        return BadRequest("Solo se pueden editar cotizaciones en estado Borrador");
                    }

                    // Actualizar campos
                    existingQuotation.ContactId = quotation.ContactId;
                    existingQuotation.ValidUntil = quotation.ValidUntil;
                    existingQuotation.DeliveryFirstName = quotation.DeliveryFirstName;
                    existingQuotation.DeliveryLastName = quotation.DeliveryLastName;
                    existingQuotation.DeliveryStreet = quotation.DeliveryStreet;
                    existingQuotation.DeliveryExteriorNumber = quotation.DeliveryExteriorNumber;
                    existingQuotation.DeliveryInteriorNumber = quotation.DeliveryInteriorNumber;
                    existingQuotation.DeliveryNeighborhood = quotation.DeliveryNeighborhood;
                    existingQuotation.DeliveryCity = quotation.DeliveryCity;
                    existingQuotation.DeliveryStateID = quotation.DeliveryStateID;
                    existingQuotation.DeliveryPostalCode = quotation.DeliveryPostalCode;
                    existingQuotation.DeliveryRFC = quotation.DeliveryRFC;
                    existingQuotation.DeliveryReference = quotation.DeliveryReference;
                    existingQuotation.Notes = quotation.Notes;
                    existingQuotation.TermsAndConditions = quotation.TermsAndConditions;
                    existingQuotation.GlobalDiscountPercentage = quotation.GlobalDiscountPercentage;
                    existingQuotation.ShippingCost = quotation.ShippingCost;
                    existingQuotation.UpdatedAt = DateTime.UtcNow;

                    // Eliminar items existentes y agregar nuevos
                    _context.QuotationItems.RemoveRange(existingQuotation.Items);

                    if (items != null && items.Any())
                    {
                        existingQuotation.Items = new List<QuotationItem>();
                        for (int i = 0; i < items.Count; i++)
                        {
                            var itemDto = items[i];
                            var quotationItem = new QuotationItem
                            {
                                ProductId = itemDto.ProductId,
                                ProductColorId = itemDto.ProductColorId,
                                Variant = itemDto.Variant,
                                Quantity = itemDto.Quantity,
                                UnitPrice = itemDto.UnitPrice,
                                DiscountedPrice = itemDto.DiscountedPrice,
                                Width = itemDto.Width,
                                Height = itemDto.Height,
                                Description = itemDto.Description,
                                DisplayOrder = i
                            };
                            existingQuotation.Items.Add(quotationItem);
                        }
                    }

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cotización actualizada exitosamente";
                    return RedirectToAction(nameof(Details), new { id = quotation.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuotationExists(quotation.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            var userStoreId = GetUserStoreId();
            PrepareViewData(userStoreId, quotation.ContactId);
            return View(quotation);
        }

        // GET: Quotation/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quotation = await _context.Quotations
                .Include(q => q.Contact)
                .Include(q => q.Store)
                .Include(q => q.CreatedByUser)
                .Include(q => q.Items)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (quotation == null)
            {
                return NotFound();
            }

            if (!CanAccessQuotation(quotation))
            {
                return Forbid();
            }

            return View(quotation);
        }

        // POST: Quotation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quotation = await _context.Quotations
                .Include(q => q.Items)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quotation != null)
            {
                if (!CanAccessQuotation(quotation))
                {
                    return Forbid();
                }

                _context.Quotations.Remove(quotation);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Cotización eliminada exitosamente";
            return RedirectToAction(nameof(Index));
        }

        // POST: Quotation/Send/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(int id)
        {
            var success = await _quotationService.ChangeStatusAsync(id, QuotationStatus.Sent, GetUserId());

            if (success)
            {
                TempData["SuccessMessage"] = "Cotización enviada exitosamente";
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo enviar la cotización. Verifica que esté en estado Borrador.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Quotation/Accept/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id)
        {
            var success = await _quotationService.ChangeStatusAsync(id, QuotationStatus.Accepted, GetUserId());

            if (success)
            {
                TempData["SuccessMessage"] = "Cotización aceptada exitosamente";
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo aceptar la cotización. Verifica que esté enviada y no expirada.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Quotation/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var success = await _quotationService.ChangeStatusAsync(id, QuotationStatus.Rejected, GetUserId());

            if (success)
            {
                TempData["SuccessMessage"] = "Cotización rechazada";
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo rechazar la cotización. Verifica que esté enviada.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Quotation/Duplicate/5
        public async Task<IActionResult> Duplicate(int id)
        {
            try
            {
                var newQuotation = await _quotationService.DuplicateQuotationAsync(id, GetUserId());
                TempData["SuccessMessage"] = "Cotización duplicada exitosamente";
                return RedirectToAction(nameof(Edit), new { id = newQuotation.Id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al duplicar: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // API: Obtener precio de producto
        [HttpGet]
        public async Task<IActionResult> GetProductPrice(int productId, int storeId, string? variant = null)
        {
            var price = await _quotationService.GetProductPriceAsync(productId, storeId, variant);

            if (price == null)
            {
                return Json(new { success = false, message = "Precio no encontrado" });
            }

            return Json(new
            {
                success = true,
                unitPrice = price.Value.unitPrice,
                discountedPrice = price.Value.discountedPrice,
                variant = price.Value.variant
            });
        }

        // API: Obtener colores de un producto
        [HttpGet]
        public async Task<IActionResult> GetProductColors(int productId)
        {
            var product = await _context.Products
                .Include(p => p.ProductColors)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return Json(new { success = false, message = "Producto no encontrado" });
            }

            var colors = product.ProductColors.Select(c => new
            {
                id = c.Id,
                name = c.Name,
                sku = c.SKU
            }).ToList();

            return Json(new { success = true, colors = colors });
        }

        // API: Obtener direcciones de un contacto
        [HttpGet]
        public async Task<IActionResult> GetContactAddresses(int contactId)
        {
            var contact = await _context.Contacts
                .Include(c => c.ShippingAddresses)
                .FirstOrDefaultAsync(c => c.Id == contactId);

            if (contact == null)
            {
                return Json(new { success = false, message = "Cliente no encontrado" });
            }

            // Preparar dirección principal del contacto
            var mainAddress = new
            {
                id = 0, // ID 0 indica la dirección principal del contacto
                label = "Dirección Principal",
                firstName = contact.FirstName,
                lastName = contact.LastName ?? "",
                street = contact.Street ?? "",
                exteriorNumber = contact.ExteriorNumber ?? "",
                interiorNumber = contact.InteriorNumber ?? "",
                neighborhood = contact.Neighborhood ?? "",
                city = contact.City ?? "",
                stateID = (int)(contact.StateID ?? MexicanState.Aguascalientes),
                postalCode = contact.PostalCode ?? "",
                rfc = contact.RFC ?? "",
                reference = ""
            };

            // Preparar direcciones de envío adicionales
            var shippingAddresses = contact.ShippingAddresses.Select(a => new
            {
                id = a.Id,
                label = $"{a.Street ?? ""} {a.ExteriorNumber ?? ""}, {a.Neighborhood ?? ""}, {a.City ?? ""}".Trim(',', ' '),
                firstName = a.FirstName,
                lastName = a.LastName,
                street = a.Street,
                exteriorNumber = a.ExteriorNumber,
                interiorNumber = a.InteriorNumber ?? "",
                neighborhood = a.Neighborhood,
                city = a.City,
                stateID = (int)a.StateID,
                postalCode = a.PostalCode,
                rfc = a.RFC ?? "",
                reference = a.DeliveryReference ?? ""
            }).ToList();

            // Combinar dirección principal con las direcciones de envío
            var allAddresses = new[] { mainAddress }.Concat(shippingAddresses).ToList();

            return Json(new { success = true, addresses = allAddresses });
        }

        // Helpers

        private bool QuotationExists(int id)
        {
            return _context.Quotations.Any(e => e.Id == id);
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : 0;
        }

        private int? GetUserStoreId()
        {
            var storeIdClaim = User.FindFirst("StoreId")?.Value;
            return int.TryParse(storeIdClaim, out int storeId) ? storeId : null;
        }

        private bool IsAdmin()
        {
            return User.IsInRole(UserRole.Administrator.ToString());
        }

        private bool CanAccessQuotation(Quotation quotation)
        {
            if (IsAdmin())
                return true;

            var userStoreId = GetUserStoreId();
            return userStoreId.HasValue && quotation.StoreId == userStoreId.Value;
        }

        private void PrepareViewData(int? storeId, int? selectedContactId = null)
        {
            // Contactos (filtrados por empresa de la tienda si no es admin)
            var contactsQuery = _context.Contacts.AsQueryable();

            if (storeId.HasValue)
            {
                var store = _context.Stores.Find(storeId.Value);
                if (store?.CompanyId != null)
                {
                    contactsQuery = contactsQuery.Where(c => c.CompanyId == store.CompanyId);
                }
            }

            var contactsList = contactsQuery
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName)
                .Select(c => new {
                    Id = c.Id,
                    FullName = c.FirstName + " " + c.LastName
                })
                .ToList();

            ViewData["ContactId"] = new SelectList(contactsList, "Id", "FullName", selectedContactId);

            // Tiendas (solo la del usuario si no es admin)
            IQueryable<Store> storesQuery = _context.Stores.Include(s => s.Company);

            if (storeId.HasValue && !IsAdmin())
            {
                storesQuery = storesQuery.Where(s => s.Id == storeId.Value);
            }

            ViewData["StoreId"] = new SelectList(storesQuery, "Id", "Name", storeId);

            // Indicar si el usuario es admin
            ViewData["IsAdmin"] = IsAdmin();

            // Guardar el nombre de la tienda para usuarios no admin
            if (storeId.HasValue)
            {
                var storeName = _context.Stores.Find(storeId.Value)?.Name;
                ViewData["StoreName"] = storeName;
            }

            // Productos (todos)
            ViewData["Products"] = _context.Products
                .Include(p => p.ProductType)
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToList();
        }
    }

    // DTO para recibir items desde el formulario
    public class QuotationItemDto
    {
        public int ProductId { get; set; }
        public int? ProductColorId { get; set; }
        public string? Variant { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public string? Description { get; set; }
    }
}
