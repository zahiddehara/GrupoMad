using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GrupoMad.Services;
using GrupoMad.Models;
using GrupoMad.Data;

namespace GrupoMad.Controllers
{
    public class PriceListController : Controller
    {
        private readonly PriceListService _priceListService;
        private readonly ApplicationDbContext _context;

        public PriceListController(PriceListService priceListService, ApplicationDbContext context)
        {
            _priceListService = priceListService;
            _context = context;
        }

        // ==================== CRUD de PriceList ====================

        // GET: PriceList
        public async Task<IActionResult> Index(bool includeInactive = false)
        {
            var priceLists = await _priceListService.GetAllPriceListsAsync(includeInactive);
            ViewBag.IncludeInactive = includeInactive;
            return View(priceLists);
        }

        // GET: PriceList/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceList = await _priceListService.GetPriceListByIdAsync(id.Value);
            if (priceList == null)
            {
                return NotFound();
            }

            return View(priceList);
        }

        // GET: PriceList/Create
        public IActionResult Create()
        {
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name");
            return View();
        }

        // POST: PriceList/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,StoreId,IsActive")] PriceList priceList)
        {
            if (ModelState.IsValid)
            {
                // Verificar que el nombre sea único
                if (!await _priceListService.IsPriceListNameUniqueAsync(priceList.Name))
                {
                    ModelState.AddModelError("Name", "Ya existe una lista de precios con este nombre.");
                    ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", priceList.StoreId);
                    return View(priceList);
                }

                await _priceListService.CreatePriceListAsync(priceList);
                TempData["Success"] = "Lista de precios creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", priceList.StoreId);
            return View(priceList);
        }

        // GET: PriceList/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceList = await _priceListService.GetPriceListByIdAsync(id.Value);
            if (priceList == null)
            {
                return NotFound();
            }

            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", priceList.StoreId);
            return View(priceList);
        }

        // POST: PriceList/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StoreId,IsActive")] PriceList priceList)
        {
            if (id != priceList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Verificar que el nombre sea único (excluyendo la lista actual)
                if (!await _priceListService.IsPriceListNameUniqueAsync(priceList.Name, priceList.Id))
                {
                    ModelState.AddModelError("Name", "Ya existe una lista de precios con este nombre.");
                    ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", priceList.StoreId);
                    return View(priceList);
                }

                var result = await _priceListService.UpdatePriceListAsync(id, priceList);
                if (result == null)
                {
                    return NotFound();
                }

                TempData["Success"] = "Lista de precios actualizada exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", priceList.StoreId);
            return View(priceList);
        }

        // GET: PriceList/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceList = await _priceListService.GetPriceListByIdAsync(id.Value);
            if (priceList == null)
            {
                return NotFound();
            }

            return View(priceList);
        }

        // POST: PriceList/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _priceListService.DeletePriceListAsync(id);
            TempData["Success"] = "Lista de precios eliminada exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // POST: PriceList/Activate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            var result = await _priceListService.ActivatePriceListAsync(id);
            if (!result)
            {
                TempData["Error"] = "No se pudo activar la lista de precios.";
            }
            else
            {
                TempData["Success"] = "Lista de precios activada exitosamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: PriceList/Deactivate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            var result = await _priceListService.DeactivatePriceListAsync(id);
            if (!result)
            {
                TempData["Error"] = "No se pudo desactivar la lista de precios.";
            }
            else
            {
                TempData["Success"] = "Lista de precios desactivada exitosamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        // ==================== Gestión de Items ====================

        // GET: PriceList/ManageItems/5
        public async Task<IActionResult> ManageItems(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceList = await _priceListService.GetPriceListByIdAsync(id.Value);
            if (priceList == null)
            {
                return NotFound();
            }

            // Obtener productos disponibles que no están en la lista
            var productsInList = priceList.PriceListItems?.Select(pli => pli.ProductId).ToList() ?? new List<int>();
            var availableProducts = _context.Products
                .Where(p => p.IsActive && !productsInList.Contains(p.Id))
                .ToList();

            ViewBag.AvailableProducts = new SelectList(availableProducts, "Id", "Name");
            ViewBag.PricingTypes = Enum.GetValues(typeof(PricingType));

            return View(priceList);
        }

        // POST: PriceList/AddItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(int priceListId, int productId, decimal price, string? variant)
        {
            try
            {
                var item = new PriceListItem
                {
                    PriceListId = priceListId,
                    ProductId = productId,
                    Price = price,
                    Variant = variant
                };

                await _priceListService.AddItemToPriceListAsync(item);
                TempData["Success"] = "Producto agregado exitosamente a la lista de precios.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(ManageItems), new { id = priceListId });
        }

        // POST: PriceList/UpdateItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateItem(int itemId, int priceListId, decimal price, string? variant)
        {
            var updatedItem = new PriceListItem
            {
                Price = price,
                Variant = variant
            };

            var result = await _priceListService.UpdatePriceListItemAsync(itemId, updatedItem);
            if (result == null)
            {
                TempData["Error"] = "No se pudo actualizar el precio del producto.";
            }
            else
            {
                TempData["Success"] = "Precio actualizado exitosamente.";
            }

            return RedirectToAction(nameof(ManageItems), new { id = priceListId });
        }

        // POST: PriceList/RemoveItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int itemId, int priceListId)
        {
            var result = await _priceListService.RemoveItemFromPriceListAsync(itemId);
            if (!result)
            {
                TempData["Error"] = "No se pudo eliminar el producto de la lista.";
            }
            else
            {
                TempData["Success"] = "Producto eliminado de la lista exitosamente.";
            }

            return RedirectToAction(nameof(ManageItems), new { id = priceListId });
        }

        // ==================== Operaciones en Lote ====================

        // GET: PriceList/BulkOperations/5
        public async Task<IActionResult> BulkOperations(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceList = await _priceListService.GetPriceListByIdAsync(id.Value);
            if (priceList == null)
            {
                return NotFound();
            }

            // Obtener otras listas de precios para copiar
            var otherPriceLists = await _priceListService.GetAllPriceListsAsync();
            ViewBag.OtherPriceLists = new SelectList(otherPriceLists.Where(pl => pl.Id != id.Value), "Id", "Name");

            return View(priceList);
        }

        // POST: PriceList/ApplyPercentage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyPercentage(int priceListId, decimal percentage)
        {
            var itemsUpdated = await _priceListService.ApplyPercentageToAllItemsAsync(priceListId, percentage);
            TempData["Success"] = $"Se aplicó {percentage}% a {itemsUpdated} productos.";
            return RedirectToAction(nameof(BulkOperations), new { id = priceListId });
        }

        // POST: PriceList/CopyItems
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CopyItems(int fromPriceListId, int toPriceListId)
        {
            var itemsCopied = await _priceListService.CopyPriceListItemsAsync(fromPriceListId, toPriceListId);
            TempData["Success"] = $"Se copiaron {itemsCopied} productos a la lista de destino.";
            return RedirectToAction(nameof(BulkOperations), new { id = toPriceListId });
        }

        // POST: PriceList/AddMultipleProducts
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMultipleProducts(int priceListId, List<int> productIds, decimal? basePrice)
        {
            var itemsAdded = await _priceListService.AddMultipleProductsAsync(priceListId, productIds, basePrice);
            TempData["Success"] = $"Se agregaron {itemsAdded} productos a la lista.";
            return RedirectToAction(nameof(ManageItems), new { id = priceListId });
        }

        // POST: PriceList/SyncProducts
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SyncProducts(int priceListId)
        {
            try
            {
                var changesCount = await _priceListService.SyncProductsToPriceListAsync(priceListId);

                if (changesCount > 0)
                {
                    TempData["Success"] = $"Sincronización completada: se realizaron {changesCount} cambio(s) (productos agregados y/o removidos).";
                }
                else
                {
                    TempData["Info"] = "La lista ya está sincronizada. No se encontraron cambios.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al sincronizar productos: {ex.Message}";
            }

            return RedirectToAction(nameof(ManageItems), new { id = priceListId });
        }

        // ==================== Vistas Especializadas ====================

        // GET: PriceList/ByStore/5
        public async Task<IActionResult> ByStore(int? storeId)
        {
            if (storeId == null)
            {
                return NotFound();
            }

            var priceLists = await _priceListService.GetPriceListsForStoreAsync(storeId.Value);
            var store = await _context.Stores.FindAsync(storeId.Value);

            ViewBag.StoreName = store?.Name;
            ViewBag.StoreId = storeId;

            return View(priceLists);
        }

        // GET: PriceList/Global
        public async Task<IActionResult> Global()
        {
            var globalPriceLists = await _priceListService.GetGlobalPriceListsAsync();
            return View(globalPriceLists);
        }

        // GET: PriceList/ProductPrices/5
        public async Task<IActionResult> ProductPrices(int? productId)
        {
            if (productId == null)
            {
                return NotFound();
            }

            var priceListItems = await _priceListService.GetPriceListsForProductAsync(productId.Value);
            var product = await _context.Products.FindAsync(productId.Value);

            ViewBag.ProductName = product?.Name;
            ViewBag.ProductId = productId;

            return View(priceListItems);
        }

        // ==================== Gestión de Descuentos ====================

        // GET: PriceList/ManageDiscounts/5
        public async Task<IActionResult> ManageDiscounts(int? itemId)
        {
            if (itemId == null)
            {
                return NotFound();
            }

            var priceListItem = await _priceListService.GetPriceListItemByIdAsync(itemId.Value);
            if (priceListItem == null)
            {
                return NotFound();
            }

            return View(priceListItem);
        }

        // POST: PriceList/AddDiscount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDiscount(int priceListItemId, decimal discountedPrice, DateTime validFrom, DateTime validUntil, int priority)
        {
            try
            {
                var discount = new PriceListItemDiscount
                {
                    PriceListItemId = priceListItemId,
                    DiscountedPrice = discountedPrice,
                    ValidFrom = validFrom,
                    ValidUntil = validUntil,
                    Priority = priority,
                    CreatedAt = DateTime.UtcNow
                };

                _context.PriceListItemDiscounts.Add(discount);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Descuento agregado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al agregar descuento: {ex.Message}";
            }

            return RedirectToAction(nameof(ManageDiscounts), new { itemId = priceListItemId });
        }

        // POST: PriceList/UpdateDiscount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDiscount(int discountId, int priceListItemId, decimal discountedPrice, DateTime validFrom, DateTime validUntil, int priority)
        {
            try
            {
                var discount = await _context.PriceListItemDiscounts.FindAsync(discountId);
                if (discount == null)
                {
                    TempData["Error"] = "Descuento no encontrado.";
                    return RedirectToAction(nameof(ManageDiscounts), new { itemId = priceListItemId });
                }

                discount.DiscountedPrice = discountedPrice;
                discount.ValidFrom = validFrom;
                discount.ValidUntil = validUntil;
                discount.Priority = priority;
                discount.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                TempData["Success"] = "Descuento actualizado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al actualizar descuento: {ex.Message}";
            }

            return RedirectToAction(nameof(ManageDiscounts), new { itemId = priceListItemId });
        }

        // POST: PriceList/DeleteDiscount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDiscount(int discountId, int priceListItemId)
        {
            try
            {
                var discount = await _context.PriceListItemDiscounts.FindAsync(discountId);
                if (discount != null)
                {
                    _context.PriceListItemDiscounts.Remove(discount);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Descuento eliminado exitosamente.";
                }
                else
                {
                    TempData["Error"] = "Descuento no encontrado.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar descuento: {ex.Message}";
            }

            return RedirectToAction(nameof(ManageDiscounts), new { itemId = priceListItemId });
        }

        // ==================== Crear PriceLists por ProductType ====================

        // GET: PriceList/CreateByProductTypes
        public async Task<IActionResult> CreateByProductTypes()
        {
            var productTypes = await _context.ProductTypes
                .Include(pt => pt.Products.Where(p => p.IsActive))
                .Where(pt => pt.IsActive)
                .OrderBy(pt => pt.Name)
                .ToListAsync();

            // Verificar cuáles ProductTypes ya tienen PriceList vinculada
            var existingProductTypeIds = await _context.PriceLists
                .Where(pl => pl.StoreId == null && pl.ProductTypeId != null) // Solo listas globales vinculadas
                .Select(pl => pl.ProductTypeId.Value)
                .ToListAsync();

            ViewBag.ExistingProductTypeIds = existingProductTypeIds;
            return View(productTypes);
        }

        // POST: PriceList/CreateByProductTypes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateByProductTypes(decimal defaultPrice = 0)
        {
            try
            {
                var productTypes = await _context.ProductTypes
                    .Include(pt => pt.Products.Where(p => p.IsActive))
                    .Where(pt => pt.IsActive)
                    .ToListAsync();

                int createdCount = 0;
                int skippedCount = 0;
                var createdLists = new List<string>();

                foreach (var productType in productTypes)
                {
                    // Verificar si ya existe una lista vinculada a este ProductType
                    var existingList = await _context.PriceLists
                        .FirstOrDefaultAsync(pl => pl.ProductTypeId == productType.Id && pl.StoreId == null);

                    if (existingList == null)
                    {
                        // Nombre de la lista basado en ProductType
                        string priceListName = $"Lista de {productType.Name}";

                        // Crear la PriceList vinculada al ProductType
                        var priceList = new PriceList
                        {
                            Name = priceListName,
                            StoreId = null, // Global
                            ProductTypeId = productType.Id, // 🔥 VINCULACIÓN AL PRODUCTTYPE
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        };

                        var createdPriceList = await _priceListService.CreatePriceListAsync(priceList);

                        // Sincronizar productos del ProductType
                        int itemsAdded = await _priceListService.SyncProductsToPriceListAsync(createdPriceList.Id);

                        createdCount++;
                        createdLists.Add($"{priceListName} ({itemsAdded} productos)");
                    }
                    else
                    {
                        skippedCount++;
                    }
                }

                if (createdCount > 0)
                {
                    TempData["Success"] = $"Se crearon {createdCount} lista(s) de precios exitosamente: {string.Join(", ", createdLists)}";
                }

                if (skippedCount > 0)
                {
                    TempData["Info"] = $"Se omitieron {skippedCount} lista(s) porque ya existían con el mismo nombre.";
                }

                if (createdCount == 0 && skippedCount == 0)
                {
                    TempData["Warning"] = "No se encontraron tipos de producto activos para crear listas.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al crear listas de precios: {ex.Message}";
                return RedirectToAction(nameof(CreateByProductTypes));
            }
        }
    }
}
