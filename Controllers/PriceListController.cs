using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using GrupoMad.Services;
using GrupoMad.Models;
using GrupoMad.Data;

namespace GrupoMad.Controllers
{
    [Authorize(Roles = "Administrator,SalesManager,StoreManager")]
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
        public async Task<IActionResult> Index(bool includeInactive = false, int? productTypeId = null, bool onlyGlobal = false, int? storeId = null)
        {
            var priceLists = await _priceListService.GetAllPriceListsAsync(includeInactive);

            // Filtrar por ProductType si se especifica
            if (productTypeId.HasValue)
            {
                priceLists = priceLists.Where(pl => pl.ProductTypeId == productTypeId.Value).ToList();
            }

            // Filtrar por Store si se especifica
            if (storeId.HasValue)
            {
                priceLists = priceLists.Where(pl => pl.StoreId == storeId.Value).ToList();
            }

            // Filtrar solo listas globales (StoreId == null)
            if (onlyGlobal)
            {
                priceLists = priceLists.Where(pl => pl.StoreId == null).ToList();
            }

            // Obtener todas las tiendas para el dropdown
            var stores = await _context.Stores.OrderBy(s => s.Name).ToListAsync();
            ViewBag.Stores = stores;

            // Obtener todos los tipos de producto para el dropdown
            var productTypes = await _context.ProductTypes
                .Where(pt => pt.IsActive)
                .OrderBy(pt => pt.Name)
                .ToListAsync();
            ViewBag.ProductTypes = new SelectList(productTypes, "Id", "Name", productTypeId);

            // Si hay un storeId seleccionado, obtener el nombre de la tienda
            if (storeId.HasValue)
            {
                var selectedStore = stores.FirstOrDefault(s => s.Id == storeId.Value);
                ViewBag.SelectedStoreName = selectedStore?.Name;
                ViewBag.SelectedStoreId = storeId.Value;
            }
            else
            {
                ViewBag.SelectedStoreId = (int?)null;
            }

            // Si hay un productTypeId seleccionado, obtener el nombre
            if (productTypeId.HasValue)
            {
                var selectedProductType = productTypes.FirstOrDefault(pt => pt.Id == productTypeId.Value);
                ViewBag.SelectedProductTypeName = selectedProductType?.Name;
                ViewBag.SelectedProductTypeId = productTypeId.Value;
            }
            else
            {
                ViewBag.SelectedProductTypeId = (int?)null;
            }

            ViewBag.IncludeInactive = includeInactive;
            ViewBag.OnlyGlobal = onlyGlobal;

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

            // Sincronizar automáticamente si la lista está vinculada a un ProductType
            if (priceList.ProductTypeId.HasValue)
            {
                try
                {
                    var changesCount = await _priceListService.SyncProductsToPriceListAsync(priceList.Id);

                    if (changesCount > 0)
                    {
                        TempData["Success"] = $"Sincronización automática completada: se realizaron {changesCount} cambio(s) (productos agregados y/o removidos).";
                        // Recargar la lista de precios después de la sincronización
                        priceList = await _priceListService.GetPriceListByIdAsync(id.Value);
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al sincronizar automáticamente: {ex.Message}";
                }
            }

            ViewBag.PricingTypes = Enum.GetValues(typeof(PricingType));

            return View(priceList);
        }

        // POST: PriceList/UpdateItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateItem(int itemId, int priceListId, decimal price, int? variantId)
        {
            var updatedItem = new PriceListItem
            {
                Price = price,
                ProductTypeVariantId = variantId
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAllVariantPrices(int priceListId, Dictionary<int, decimal> prices)
        {
            try
            {
                int updated = 0;
                foreach (var kvp in prices)
                {
                    var itemId = kvp.Key;
                    var newPrice = kvp.Value;

                    var item = await _context.PriceListItems
                        .Include(pli => pli.Discounts)
                        .FirstOrDefaultAsync(pli => pli.Id == itemId);

                    if (item != null && item.PriceListId == priceListId)
                    {
                        var oldPrice = item.Price;
                        item.Price = newPrice;
                        item.UpdatedAt = DateTime.UtcNow;
                        updated++;

                        // Recalcular descuentos existentes para mantener el mismo porcentaje
                        if (item.Discounts != null && item.Discounts.Any() && oldPrice > 0)
                        {
                            foreach (var discount in item.Discounts)
                            {
                                // Calcular el porcentaje de descuento original
                                var discountPercent = ((oldPrice - discount.DiscountedPrice) / oldPrice) * 100;

                                // Aplicar el mismo porcentaje al nuevo precio
                                discount.DiscountedPrice = newPrice * (1 - (discountPercent / 100));
                                discount.UpdatedAt = DateTime.UtcNow;
                            }
                        }
                    }
                }

                if (updated > 0)
                {
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Se actualizaron {updated} precio(s) exitosamente.";
                }
                else
                {
                    TempData["Info"] = "No se realizaron cambios.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al guardar los precios: {ex.Message}";
            }

            return RedirectToAction(nameof(ManageItems), new { id = priceListId });
        }

        // ==================== Operaciones en Lote ====================

        // POST: PriceList/ApplyPercentageFromGlobal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyPercentageFromGlobal(int priceListId, decimal percentage)
        {
            try
            {
                // Obtener la lista de precios de tienda
                var storePriceList = await _context.PriceLists
                    .Include(pl => pl.PriceListItems)
                        .ThenInclude(pli => pli.Product)
                    .FirstOrDefaultAsync(pl => pl.Id == priceListId);

                if (storePriceList == null)
                {
                    TempData["Error"] = "Lista de precios no encontrada.";
                    return RedirectToAction(nameof(ManageItems), new { id = priceListId });
                }

                if (!storePriceList.StoreId.HasValue)
                {
                    TempData["Error"] = "Esta función solo está disponible para listas de tienda.";
                    return RedirectToAction(nameof(ManageItems), new { id = priceListId });
                }

                if (!storePriceList.ProductTypeId.HasValue)
                {
                    TempData["Error"] = "Esta lista no está vinculada a un tipo de producto.";
                    return RedirectToAction(nameof(ManageItems), new { id = priceListId });
                }

                // Buscar la lista global correspondiente
                var globalPriceList = await _context.PriceLists
                    .Include(pl => pl.PriceListItems)
                    .FirstOrDefaultAsync(pl =>
                        pl.ProductTypeId == storePriceList.ProductTypeId &&
                        pl.StoreId == null);

                if (globalPriceList == null)
                {
                    TempData["Error"] = "No se encontró la lista global correspondiente a este tipo de producto.";
                    return RedirectToAction(nameof(ManageItems), new { id = priceListId });
                }

                int itemsUpdated = 0;
                int itemsNotFound = 0;

                // Actualizar cada item de la lista de tienda con el porcentaje aplicado al precio global
                foreach (var storeItem in storePriceList.PriceListItems)
                {
                    // Buscar el precio del mismo producto en la lista global
                    var globalItem = globalPriceList.PriceListItems
                        .FirstOrDefault(gi => gi.ProductId == storeItem.ProductId);

                    if (globalItem != null)
                    {
                        // Calcular nuevo precio: PrecioGlobal * (1 + Porcentaje/100)
                        decimal newPrice = globalItem.Price * (1 + (percentage / 100));
                        storeItem.Price = Math.Round(newPrice, 2);
                        storeItem.UpdatedAt = DateTime.UtcNow;
                        itemsUpdated++;
                    }
                    else
                    {
                        itemsNotFound++;
                    }
                }

                await _context.SaveChangesAsync();

                if (itemsUpdated > 0)
                {
                    TempData["Success"] = $"Se actualizaron {itemsUpdated} producto(s) aplicando {percentage:+0.##;-0.##}% sobre los precios de la lista global.";
                }

                if (itemsNotFound > 0)
                {
                    TempData["Info"] = $"{itemsNotFound} producto(s) no se encontraron en la lista global y no fueron actualizados.";
                }

                return RedirectToAction(nameof(ManageItems), new { id = priceListId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al aplicar porcentaje: {ex.Message}";
                return RedirectToAction(nameof(ManageItems), new { id = priceListId });
            }
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

        // POST: PriceList/ApplyPercentageDiscountToVariants
        [HttpPost]
        public async Task<IActionResult> ApplyPercentageDiscountToVariants(string itemIds, decimal discountPercent)
        {
            try
            {
                var itemIdList = itemIds.Split(',').Select(int.Parse).ToList();

                // Si el descuento es 0 o negativo, eliminar todos los descuentos de estos items
                if (discountPercent <= 0)
                {
                    var discountsToRemove = await _context.PriceListItemDiscounts
                        .Where(d => itemIdList.Contains(d.PriceListItemId))
                        .ToListAsync();

                    _context.PriceListItemDiscounts.RemoveRange(discountsToRemove);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Descuentos eliminados" });
                }

                // Obtener todos los items
                var items = await _context.PriceListItems
                    .Where(pli => itemIdList.Contains(pli.Id))
                    .ToListAsync();

                foreach (var item in items)
                {
                    // Calcular el precio con descuento
                    var discountedPrice = item.Price * (1 - (discountPercent / 100));

                    // Buscar si ya existe un descuento para este item
                    var existingDiscount = await _context.PriceListItemDiscounts
                        .FirstOrDefaultAsync(d => d.PriceListItemId == item.Id);

                    if (existingDiscount != null)
                    {
                        // Actualizar descuento existente
                        existingDiscount.DiscountedPrice = discountedPrice;
                        existingDiscount.ValidFrom = DateTime.UtcNow.AddDays(-1); // Vigente desde ayer
                        existingDiscount.ValidUntil = DateTime.MaxValue; // Sin fecha de expiración
                        existingDiscount.Priority = 1;
                        existingDiscount.UpdatedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        // Crear nuevo descuento
                        var newDiscount = new PriceListItemDiscount
                        {
                            PriceListItemId = item.Id,
                            DiscountedPrice = discountedPrice,
                            ValidFrom = DateTime.UtcNow.AddDays(-1),
                            ValidUntil = DateTime.MaxValue,
                            Priority = 1,
                            CreatedAt = DateTime.UtcNow
                        };
                        _context.PriceListItemDiscounts.Add(newDiscount);
                    }
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = $"Descuento del {discountPercent}% aplicado a {items.Count} variante(s)" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
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

        // ==================== Crear PriceLists por ProductType y Store ====================

        // GET: PriceList/CreateByProductTypesForStores
        public async Task<IActionResult> CreateByProductTypesForStores()
        {
            var productTypes = await _context.ProductTypes
                .Include(pt => pt.Products.Where(p => p.IsActive))
                .Where(pt => pt.IsActive)
                .OrderBy(pt => pt.Name)
                .ToListAsync();

            var stores = await _context.Stores
                .OrderBy(s => s.Name)
                .ToListAsync();

            // Verificar cuáles combinaciones de Store + ProductType ya tienen PriceList
            var existingCombinations = await _context.PriceLists
                .Where(pl => pl.StoreId != null && pl.ProductTypeId != null)
                .Select(pl => new { pl.StoreId, pl.ProductTypeId })
                .ToListAsync();

            ViewBag.Stores = stores;
            ViewBag.ExistingCombinations = existingCombinations
                .Select(ec => (ec.StoreId.Value, ec.ProductTypeId.Value))
                .ToList();

            return View(productTypes);
        }

        // POST: PriceList/CreateByProductTypesForStores
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateByProductTypesForStores(decimal defaultPrice = 0)
        {
            try
            {
                var productTypes = await _context.ProductTypes
                    .Include(pt => pt.Products.Where(p => p.IsActive))
                    .Where(pt => pt.IsActive)
                    .ToListAsync();

                var stores = await _context.Stores
                    .ToListAsync();

                int createdCount = 0;
                int skippedCount = 0;
                var createdLists = new List<string>();

                foreach (var store in stores)
                {
                    foreach (var productType in productTypes)
                    {
                        // Verificar si ya existe una lista para esta combinación Store + ProductType
                        var existingList = await _context.PriceLists
                            .FirstOrDefaultAsync(pl =>
                                pl.ProductTypeId == productType.Id &&
                                pl.StoreId == store.Id);

                        if (existingList == null && productType.Products.Count > 0)
                        {
                            // Nombre de la lista basado en ProductType y Store
                            string priceListName = $"Lista de {productType.Name} - {store.Name}";

                            // Crear la PriceList vinculada al ProductType y Store
                            var priceList = new PriceList
                            {
                                Name = priceListName,
                                StoreId = store.Id,
                                ProductTypeId = productType.Id,
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
                }

                if (createdCount > 0)
                {
                    TempData["Success"] = $"Se crearon {createdCount} lista(s) de precios exitosamente.";
                }

                if (skippedCount > 0)
                {
                    TempData["Info"] = $"Se omitieron {skippedCount} combinación(es) porque ya existían o no tenían productos.";
                }

                if (createdCount == 0 && skippedCount == 0)
                {
                    TempData["Warning"] = "No se encontraron tipos de producto o tiendas activas para crear listas.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al crear listas de precios: {ex.Message}";
                return RedirectToAction(nameof(CreateByProductTypesForStores));
            }
        }

        #region Gestión de Rangos de Precio

        // GET: PriceList/ManageRangesByLength/5
        public async Task<IActionResult> ManageRangesByLength(int? itemId)
        {
            if (itemId == null)
            {
                return NotFound();
            }

            var item = await _context.PriceListItems
                .Include(pli => pli.Product)
                    .ThenInclude(p => p.ProductType)
                .Include(pli => pli.PriceList)
                .Include(pli => pli.PriceRangesByLength)
                .FirstOrDefaultAsync(pli => pli.Id == itemId);

            if (item == null)
            {
                return NotFound();
            }

            // Verificar que el producto sea del tipo PerRangeLength
            if (item.Product?.ProductType?.PricingType != PricingType.PerRangeLength)
            {
                TempData["Error"] = "Este producto no es del tipo 'Por Rango de Largo'";
                return RedirectToAction(nameof(ManageItems), new { id = item.PriceListId });
            }

            return View(item);
        }

        // POST: PriceList/AddRangeByLength
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRangeByLength(int priceListItemId, decimal minLength, decimal maxLength, decimal price)
        {
            try
            {
                // Validar que min < max
                if (minLength >= maxLength)
                {
                    TempData["Error"] = "El largo mínimo debe ser menor que el largo máximo";
                    return RedirectToAction(nameof(ManageRangesByLength), new { itemId = priceListItemId });
                }

                // Validar que no se solape con rangos existentes
                var existingRanges = await _context.PriceRangesByLength
                    .Where(r => r.PriceListItemId == priceListItemId)
                    .ToListAsync();

                var overlaps = existingRanges.Any(r =>
                    (minLength >= r.MinLength && minLength <= r.MaxLength) ||
                    (maxLength >= r.MinLength && maxLength <= r.MaxLength) ||
                    (minLength <= r.MinLength && maxLength >= r.MaxLength)
                );

                if (overlaps)
                {
                    TempData["Error"] = "El rango se solapa con un rango existente. Verifica que los valores no se traslapen con rangos ya configurados.";
                    return RedirectToAction(nameof(ManageRangesByLength), new { itemId = priceListItemId });
                }

                var range = new PriceRangeByLength
                {
                    PriceListItemId = priceListItemId,
                    MinLength = minLength,
                    MaxLength = maxLength,
                    Price = price
                };

                _context.PriceRangesByLength.Add(range);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Rango agregado exitosamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al agregar rango: {ex.Message}";
            }

            return RedirectToAction(nameof(ManageRangesByLength), new { itemId = priceListItemId });
        }

        // POST: PriceList/UpdateRangeByLength
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRangeByLength(int rangeId, int priceListItemId, decimal minLength, decimal maxLength, decimal price)
        {
            try
            {
                // Validar que min < max
                if (minLength >= maxLength)
                {
                    TempData["Error"] = "El largo mínimo debe ser menor que el largo máximo";
                    return RedirectToAction(nameof(ManageRangesByLength), new { itemId = priceListItemId });
                }

                // Validar que no se solape con otros rangos (excluyendo el que estamos editando)
                var existingRanges = await _context.PriceRangesByLength
                    .Where(r => r.PriceListItemId == priceListItemId && r.Id != rangeId)
                    .ToListAsync();

                var overlaps = existingRanges.Any(r =>
                    (minLength >= r.MinLength && minLength <= r.MaxLength) ||
                    (maxLength >= r.MinLength && maxLength <= r.MaxLength) ||
                    (minLength <= r.MinLength && maxLength >= r.MaxLength)
                );

                if (overlaps)
                {
                    TempData["Error"] = "El rango se solapa con un rango existente. Verifica que los valores no se traslapen con rangos ya configurados.";
                    return RedirectToAction(nameof(ManageRangesByLength), new { itemId = priceListItemId });
                }

                var range = await _context.PriceRangesByLength.FindAsync(rangeId);
                if (range != null)
                {
                    range.MinLength = minLength;
                    range.MaxLength = maxLength;
                    range.Price = price;
                    range.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Rango actualizado exitosamente";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al actualizar rango: {ex.Message}";
            }

            return RedirectToAction(nameof(ManageRangesByLength), new { itemId = priceListItemId });
        }

        // POST: PriceList/DeleteRangeByLength
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRangeByLength(int rangeId, int priceListItemId)
        {
            try
            {
                var range = await _context.PriceRangesByLength.FindAsync(rangeId);
                if (range != null)
                {
                    _context.PriceRangesByLength.Remove(range);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Rango eliminado exitosamente";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar rango: {ex.Message}";
            }

            return RedirectToAction(nameof(ManageRangesByLength), new { itemId = priceListItemId });
        }

        // GET: PriceList/ManageRangesByDimensions/5
        public async Task<IActionResult> ManageRangesByDimensions(int? itemId)
        {
            if (itemId == null)
            {
                return NotFound();
            }

            var item = await _context.PriceListItems
                .Include(pli => pli.Product)
                    .ThenInclude(p => p.ProductType)
                .Include(pli => pli.PriceList)
                .Include(pli => pli.PriceRangesByDimensions)
                .FirstOrDefaultAsync(pli => pli.Id == itemId);

            if (item == null)
            {
                return NotFound();
            }

            // Verificar que el producto sea del tipo PerRangeDimensions
            if (item.Product?.ProductType?.PricingType != PricingType.PerRangeDimensions)
            {
                TempData["Error"] = "Este producto no es del tipo 'Por Rango de Dimensiones'";
                return RedirectToAction(nameof(ManageItems), new { id = item.PriceListId });
            }

            return View(item);
        }

        // POST: PriceList/AddRangeByDimensions
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRangeByDimensions(int priceListItemId, decimal minWidth, decimal maxWidth, decimal minHeight, decimal maxHeight, decimal price)
        {
            try
            {
                // Validar que min < max
                if (minWidth >= maxWidth)
                {
                    TempData["Error"] = "El ancho mínimo debe ser menor que el ancho máximo";
                    return RedirectToAction(nameof(ManageRangesByDimensions), new { itemId = priceListItemId });
                }

                if (minHeight >= maxHeight)
                {
                    TempData["Error"] = "El alto mínimo debe ser menor que el alto máximo";
                    return RedirectToAction(nameof(ManageRangesByDimensions), new { itemId = priceListItemId });
                }

                // Validar que no se solape con rangos existentes
                var existingRanges = await _context.PriceRangesByDimensions
                    .Where(r => r.PriceListItemId == priceListItemId)
                    .ToListAsync();

                var overlaps = existingRanges.Any(r =>
                    // Verifica si hay solapamiento en ambas dimensiones (ancho Y alto)
                    ((minWidth >= r.MinWidth && minWidth <= r.MaxWidth) || (maxWidth >= r.MinWidth && maxWidth <= r.MaxWidth) || (minWidth <= r.MinWidth && maxWidth >= r.MaxWidth)) &&
                    ((minHeight >= r.MinHeight && minHeight <= r.MaxHeight) || (maxHeight >= r.MinHeight && maxHeight <= r.MaxHeight) || (minHeight <= r.MinHeight && maxHeight >= r.MaxHeight))
                );

                if (overlaps)
                {
                    TempData["Error"] = "El rango se solapa con un rango existente. Verifica que los valores no se traslapen con rangos ya configurados.";
                    return RedirectToAction(nameof(ManageRangesByDimensions), new { itemId = priceListItemId });
                }

                var range = new PriceRangeByDimensions
                {
                    PriceListItemId = priceListItemId,
                    MinWidth = minWidth,
                    MaxWidth = maxWidth,
                    MinHeight = minHeight,
                    MaxHeight = maxHeight,
                    Price = price
                };

                _context.PriceRangesByDimensions.Add(range);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Rango agregado exitosamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al agregar rango: {ex.Message}";
            }

            return RedirectToAction(nameof(ManageRangesByDimensions), new { itemId = priceListItemId });
        }

        // POST: PriceList/UpdateRangeByDimensions
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRangeByDimensions(int rangeId, int priceListItemId, decimal minWidth, decimal maxWidth, decimal minHeight, decimal maxHeight, decimal price)
        {
            try
            {
                // Validar que min < max
                if (minWidth >= maxWidth)
                {
                    TempData["Error"] = "El ancho mínimo debe ser menor que el ancho máximo";
                    return RedirectToAction(nameof(ManageRangesByDimensions), new { itemId = priceListItemId });
                }

                if (minHeight >= maxHeight)
                {
                    TempData["Error"] = "El alto mínimo debe ser menor que el alto máximo";
                    return RedirectToAction(nameof(ManageRangesByDimensions), new { itemId = priceListItemId });
                }

                // Validar que no se solape con otros rangos (excluyendo el que estamos editando)
                var existingRanges = await _context.PriceRangesByDimensions
                    .Where(r => r.PriceListItemId == priceListItemId && r.Id != rangeId)
                    .ToListAsync();

                var overlaps = existingRanges.Any(r =>
                    // Verifica si hay solapamiento en ambas dimensiones (ancho Y alto)
                    ((minWidth >= r.MinWidth && minWidth <= r.MaxWidth) || (maxWidth >= r.MinWidth && maxWidth <= r.MaxWidth) || (minWidth <= r.MinWidth && maxWidth >= r.MaxWidth)) &&
                    ((minHeight >= r.MinHeight && minHeight <= r.MaxHeight) || (maxHeight >= r.MinHeight && maxHeight <= r.MaxHeight) || (minHeight <= r.MinHeight && maxHeight >= r.MaxHeight))
                );

                if (overlaps)
                {
                    TempData["Error"] = "El rango se solapa con un rango existente. Verifica que los valores no se traslapen con rangos ya configurados.";
                    return RedirectToAction(nameof(ManageRangesByDimensions), new { itemId = priceListItemId });
                }

                var range = await _context.PriceRangesByDimensions.FindAsync(rangeId);
                if (range != null)
                {
                    range.MinWidth = minWidth;
                    range.MaxWidth = maxWidth;
                    range.MinHeight = minHeight;
                    range.MaxHeight = maxHeight;
                    range.Price = price;
                    range.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Rango actualizado exitosamente";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al actualizar rango: {ex.Message}";
            }

            return RedirectToAction(nameof(ManageRangesByDimensions), new { itemId = priceListItemId });
        }

        // POST: PriceList/DeleteRangeByDimensions
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRangeByDimensions(int rangeId, int priceListItemId)
        {
            try
            {
                var range = await _context.PriceRangesByDimensions.FindAsync(rangeId);
                if (range != null)
                {
                    _context.PriceRangesByDimensions.Remove(range);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Rango eliminado exitosamente";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar rango: {ex.Message}";
            }

            return RedirectToAction(nameof(ManageRangesByDimensions), new { itemId = priceListItemId });
        }

        // GET: PriceList/ManageRangesByDimensionsMatrix/5
        public async Task<IActionResult> ManageRangesByDimensionsMatrix(int? itemId)
        {
            if (itemId == null)
            {
                return NotFound();
            }

            var item = await _context.PriceListItems
                .Include(pli => pli.Product)
                    .ThenInclude(p => p.ProductType)
                .Include(pli => pli.PriceList)
                .Include(pli => pli.PriceRangesByDimensions)
                .FirstOrDefaultAsync(pli => pli.Id == itemId);

            if (item == null)
            {
                return NotFound();
            }

            // Verify product type is PerRangeDimensions
            if (item.Product?.ProductType?.PricingType != PricingType.PerRangeDimensions)
            {
                throw new InvalidOperationException("Este tipo de producto no soporta rangos por dimensiones");
            }

            // Build a dictionary of existing prices indexed by range combination
            var existingPrices = new Dictionary<string, (int Id, decimal Price)>();
            foreach (var range in item.PriceRangesByDimensions)
            {
                // Find matching predefined ranges
                var widthIndex = Helpers.DimensionRanges.WidthRanges
                    .FindIndex(r => r.Min == range.MinWidth && r.Max == range.MaxWidth);
                var lengthIndex = Helpers.DimensionRanges.LengthRanges
                    .FindIndex(r => r.Min == range.MinHeight && r.Max == range.MaxHeight);

                if (widthIndex >= 0 && lengthIndex >= 0)
                {
                    var key = Helpers.DimensionRanges.CreateRangeKey(widthIndex, lengthIndex);
                    existingPrices[key] = (range.Id, range.Price);
                }
            }

            ViewBag.ExistingPrices = existingPrices;
            return View(item);
        }

        // POST: PriceList/SaveRangesByDimensionsMatrix
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> SaveRangesByDimensionsMatrix([FromBody] SaveRangesByDimensionsRequest request)
        {
            try
            {
                var item = await _context.PriceListItems
                    .Include(pli => pli.PriceRangesByDimensions)
                    .FirstOrDefaultAsync(pli => pli.Id == request.ItemId);

                if (item == null)
                {
                    return NotFound();
                }

                int created = 0, updated = 0;

                foreach (var kvp in request.Prices)
                {
                    var (widthIndex, lengthIndex) = Helpers.DimensionRanges.ParseRangeKey(kvp.Key);
                    var price = kvp.Value;

                    // Skip if price is 0 or negative
                    if (price <= 0)
                        continue;

                    var widthRange = Helpers.DimensionRanges.WidthRanges[widthIndex];
                    var lengthRange = Helpers.DimensionRanges.LengthRanges[lengthIndex];

                    // Check if range already exists
                    var existingRange = item.PriceRangesByDimensions
                        .FirstOrDefault(r =>
                            r.MinWidth == widthRange.Min &&
                            r.MaxWidth == widthRange.Max &&
                            r.MinHeight == lengthRange.Min &&
                            r.MaxHeight == lengthRange.Max);

                    if (existingRange != null)
                    {
                        // Update existing
                        existingRange.Price = price;
                        existingRange.UpdatedAt = DateTime.Now;
                        updated++;
                    }
                    else
                    {
                        // Create new
                        var newRange = new PriceRangeByDimensions
                        {
                            PriceListItemId = request.ItemId,
                            MinWidth = widthRange.Min,
                            MaxWidth = widthRange.Max,
                            MinHeight = lengthRange.Min,
                            MaxHeight = lengthRange.Max,
                            Price = price,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };
                        _context.PriceRangesByDimensions.Add(newRange);
                        created++;
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = $"Matriz guardada exitosamente. Creados: {created}, Actualizados: {updated}";

                return Json(new { success = true, message = TempData["Success"] });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al guardar matriz: {ex.Message}" });
            }
        }

        #endregion

        #region Curtain Pricing

        // GET: PriceList/ManageCurtainPricing/5
        public async Task<IActionResult> ManageCurtainPricing(int? itemId)
        {
            if (itemId == null)
            {
                return NotFound();
            }

            var item = await _context.PriceListItems
                .Include(pli => pli.Product)
                    .ThenInclude(p => p.ProductType)
                .Include(pli => pli.PriceList)
                .Include(pli => pli.ProductTypeHeadingStyle)
                .Include(pli => pli.CurtainPricingConfig)
                .FirstOrDefaultAsync(pli => pli.Id == itemId.Value);

            if (item == null)
            {
                return NotFound();
            }

            // Verify this is a curtain product with heading styles
            if (item.Product?.ProductType?.HasHeadingStyles != true)
            {
                TempData["Error"] = "Este producto no tiene estilos de cabecera configurados.";
                return RedirectToAction(nameof(ManageItems), new { id = item.PriceListId });
            }

            return View(item);
        }

        // POST: PriceList/SaveCurtainPricing
        [HttpPost]
        public async Task<IActionResult> SaveCurtainPricing([FromBody] SaveCurtainPricingRequest request)
        {
            try
            {
                var item = await _context.PriceListItems
                    .Include(pli => pli.CurtainPricingConfig)
                    .Include(pli => pli.PriceRangesByDimensions)
                    .FirstOrDefaultAsync(pli => pli.Id == request.ItemId);

                if (item == null)
                {
                    return Json(new { success = false, message = "Item no encontrado" });
                }

                // Save or update curtain pricing config
                if (item.CurtainPricingConfig == null)
                {
                    item.CurtainPricingConfig = new CurtainPricingConfig
                    {
                        PriceListItemId = item.Id,
                        BasePrice = request.BasePrice,
                        TaxPercent = request.TaxPercent,
                        ProfitMarginsByHeightJson = System.Text.Json.JsonSerializer.Serialize(request.ProfitMargins),
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.CurtainPricingConfigs.Add(item.CurtainPricingConfig);
                }
                else
                {
                    item.CurtainPricingConfig.BasePrice = request.BasePrice;
                    item.CurtainPricingConfig.TaxPercent = request.TaxPercent;
                    item.CurtainPricingConfig.ProfitMarginsByHeightJson = System.Text.Json.JsonSerializer.Serialize(request.ProfitMargins);
                    item.CurtainPricingConfig.UpdatedAt = DateTime.UtcNow;
                }

                // Calculate and save prices for each dimension range
                var widthRanges = GrupoMad.Helpers.DimensionRanges.WidthRanges;
                var heightRanges = GrupoMad.Helpers.DimensionRanges.LengthRanges;

                int created = 0;
                int updated = 0;

                for (int h = 0; h < heightRanges.Count; h++)
                {
                    var heightRange = heightRanges[h];
                    var profitMargin = request.ProfitMargins.ContainsKey(h.ToString())
                        ? request.ProfitMargins[h.ToString()]
                        : 0;

                    for (int w = 0; w < widthRanges.Count; w++)
                    {
                        var widthRange = widthRanges[w];

                        // Get fabric usage from the matrix
                        var fabricUsage = GrupoMad.Helpers.CurtainFabricMatrix.GetFabricUsageByRangeIndex(w, h);

                        // Calculate price using formula: (BasePrice * FabricUsage) * ((ProfitMargin * 0.01) + 1) * ((Tax * 0.01) + 1)
                        var price = (request.BasePrice * fabricUsage)
                                  * ((profitMargin * 0.01m) + 1)
                                  * ((request.TaxPercent * 0.01m) + 1);

                        // Round to 2 decimal places
                        price = Math.Round(price, 2);

                        // Find or create price range
                        var existingRange = item.PriceRangesByDimensions?.FirstOrDefault(r =>
                            r.MinWidth == widthRange.Min &&
                            r.MaxWidth == widthRange.Max &&
                            r.MinHeight == heightRange.Min &&
                            r.MaxHeight == heightRange.Max);

                        if (existingRange != null)
                        {
                            existingRange.Price = price;
                            existingRange.UpdatedAt = DateTime.UtcNow;
                            updated++;
                        }
                        else
                        {
                            var newRange = new PriceRangeByDimensions
                            {
                                PriceListItemId = item.Id,
                                MinWidth = widthRange.Min,
                                MaxWidth = widthRange.Max,
                                MinHeight = heightRange.Min,
                                MaxHeight = heightRange.Max,
                                Price = price,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };
                            _context.PriceRangesByDimensions.Add(newRange);
                            created++;
                        }
                    }
                }

                await _context.SaveChangesAsync();

                return Json(new {
                    success = true,
                    message = $"Configuración guardada y precios calculados. Creados: {created}, Actualizados: {updated}"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        #endregion

        #region Special Curtain Pricing

        // GET: PriceList/ManageSpecialCurtainPricing/5
        public async Task<IActionResult> ManageSpecialCurtainPricing(int? itemId)
        {
            if (itemId == null)
            {
                return NotFound();
            }

            var item = await _context.PriceListItems
                .Include(pli => pli.Product)
                    .ThenInclude(p => p.ProductType)
                .Include(pli => pli.PriceList)
                .Include(pli => pli.ProductTypeHeadingStyle)
                .Include(pli => pli.CurtainPricingConfig)
                .FirstOrDefaultAsync(pli => pli.Id == itemId.Value);

            if (item == null)
            {
                return NotFound();
            }

            // Verify this is a curtain product with heading styles
            if (item.Product?.ProductType?.HasHeadingStyles != true)
            {
                TempData["Error"] = "Este producto no tiene estilos de cabecera configurados.";
                return RedirectToAction(nameof(ManageItems), new { id = item.PriceListId });
            }

            return View(item);
        }

        // POST: PriceList/SaveSpecialCurtainPricing
        [HttpPost]
        public async Task<IActionResult> SaveSpecialCurtainPricing([FromBody] SaveSpecialCurtainPricingRequest request)
        {
            try
            {
                var item = await _context.PriceListItems
                    .Include(pli => pli.CurtainPricingConfig)
                    .Include(pli => pli.PriceRangesByDimensions)
                    .FirstOrDefaultAsync(pli => pli.Id == request.ItemId);

                if (item == null)
                {
                    return Json(new { success = false, message = "Item no encontrado" });
                }

                // Save or update curtain pricing config with Special type
                if (item.CurtainPricingConfig == null)
                {
                    item.CurtainPricingConfig = new CurtainPricingConfig
                    {
                        PriceListItemId = item.Id,
                        BasePrice = request.BasePrice,
                        TaxPercent = request.TaxPercent,
                        PricingType = CurtainPricingType.Special,
                        ProfitMarginsByHeightJson = System.Text.Json.JsonSerializer.Serialize(request.ProfitMargins),
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.CurtainPricingConfigs.Add(item.CurtainPricingConfig);
                }
                else
                {
                    item.CurtainPricingConfig.BasePrice = request.BasePrice;
                    item.CurtainPricingConfig.TaxPercent = request.TaxPercent;
                    item.CurtainPricingConfig.PricingType = CurtainPricingType.Special;
                    item.CurtainPricingConfig.ProfitMarginsByHeightJson = System.Text.Json.JsonSerializer.Serialize(request.ProfitMargins);
                    item.CurtainPricingConfig.UpdatedAt = DateTime.UtcNow;
                }

                // Calculate and save prices for each dimension range (29 widths × 6 heights)
                var widthRanges = GrupoMad.Helpers.DimensionRanges.WidthRanges;
                var heightRanges = GrupoMad.Helpers.DimensionRanges.SpecialLengthRanges;

                int created = 0;
                int updated = 0;

                // Remove existing ranges before creating new ones (only one pricing type at a time)
                if (item.PriceRangesByDimensions != null && item.PriceRangesByDimensions.Any())
                {
                    _context.PriceRangesByDimensions.RemoveRange(item.PriceRangesByDimensions);
                }

                for (int h = 0; h < heightRanges.Count; h++)
                {
                    var heightRange = heightRanges[h];
                    var profitMargin = request.ProfitMargins.ContainsKey(h.ToString())
                        ? request.ProfitMargins[h.ToString()]
                        : 0;

                    for (int w = 0; w < widthRanges.Count; w++)
                    {
                        var widthRange = widthRanges[w];

                        // Get fabric usage from the SPECIAL matrix
                        var fabricUsage = GrupoMad.Helpers.SpecialCurtainFabricMatrix.GetFabricUsageByRangeIndex(w, h);

                        // Calculate price using formula: (BasePrice * FabricUsage) * ((ProfitMargin * 0.01) + 1) * ((Tax * 0.01) + 1)
                        var price = (request.BasePrice * fabricUsage)
                                  * ((profitMargin * 0.01m) + 1)
                                  * ((request.TaxPercent * 0.01m) + 1);

                        // Round to 2 decimal places
                        price = Math.Round(price, 2);

                        // Create new price range
                        var newRange = new PriceRangeByDimensions
                        {
                            PriceListItemId = item.Id,
                            MinWidth = widthRange.Min,
                            MaxWidth = widthRange.Max,
                            MinHeight = heightRange.Min,
                            MaxHeight = heightRange.Max,
                            Price = price,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _context.PriceRangesByDimensions.Add(newRange);
                        created++;
                    }
                }

                await _context.SaveChangesAsync();

                return Json(new {
                    success = true,
                    message = $"Configuración guardada exitosamente. Se crearon {created} precios (29 anchos × 6 alturas)."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        #endregion
    }

    // Request model for SaveRangesByDimensionsMatrix
    public class SaveRangesByDimensionsRequest
    {
        public int ItemId { get; set; }
        public Dictionary<string, decimal> Prices { get; set; } = new Dictionary<string, decimal>();
    }

    // Request model for SaveCurtainPricing
    public class SaveCurtainPricingRequest
    {
        public int ItemId { get; set; }
        public decimal BasePrice { get; set; }
        public decimal TaxPercent { get; set; }
        public Dictionary<string, decimal> ProfitMargins { get; set; } = new Dictionary<string, decimal>();
    }

    // Request model for SaveSpecialCurtainPricing
    public class SaveSpecialCurtainPricingRequest
    {
        public int ItemId { get; set; }
        public decimal BasePrice { get; set; }
        public decimal TaxPercent { get; set; }
        public Dictionary<string, decimal> ProfitMargins { get; set; } = new Dictionary<string, decimal>();
    }
}
