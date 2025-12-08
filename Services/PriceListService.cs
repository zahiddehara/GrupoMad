using GrupoMad.Data;
using GrupoMad.Models;
using Microsoft.EntityFrameworkCore;

namespace GrupoMad.Services
{
    public class PriceListService
    {
        private readonly ApplicationDbContext _context;

        public PriceListService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==================== CRUD de PriceList ====================

        public async Task<List<PriceList>> GetAllPriceListsAsync(bool includeInactive = false)
        {
            var query = _context.PriceLists
                .Include(pl => pl.Store)
                .Include(pl => pl.ProductType)
                .Include(pl => pl.PriceListItems)
                    .ThenInclude(pli => pli.Product)
                .Include(pl => pl.PriceListItems)
                    .ThenInclude(pli => pli.Discounts)
                .AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(pl => pl.IsActive);
            }

            return await query.OrderBy(pl => pl.Name).ToListAsync();
        }

        public async Task<PriceList?> GetPriceListByIdAsync(int id)
        {
            return await _context.PriceLists
                .Include(pl => pl.Store)
                .Include(pl => pl.ProductType)
                .Include(pl => pl.PriceListItems)
                    .ThenInclude(pli => pli.Product)
                        .ThenInclude(p => p.ProductColors)
                .Include(pl => pl.PriceListItems)
                    .ThenInclude(pli => pli.Product)
                        .ThenInclude(p => p.ProductType)
                .Include(pl => pl.PriceListItems)
                    .ThenInclude(pli => pli.Discounts)
                .FirstOrDefaultAsync(pl => pl.Id == id);
        }

        public async Task<PriceList> CreatePriceListAsync(PriceList priceList)
        {
            priceList.CreatedAt = DateTime.UtcNow;
            _context.PriceLists.Add(priceList);
            await _context.SaveChangesAsync();
            return priceList;
        }

        public async Task<PriceList?> UpdatePriceListAsync(int id, PriceList updatedPriceList)
        {
            var priceList = await _context.PriceLists.FindAsync(id);
            if (priceList == null) return null;

            priceList.Name = updatedPriceList.Name;
            priceList.StoreId = updatedPriceList.StoreId;
            priceList.IsActive = updatedPriceList.IsActive;
            priceList.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return priceList;
        }

        public async Task<bool> DeletePriceListAsync(int id)
        {
            var priceList = await _context.PriceLists.FindAsync(id);
            if (priceList == null) return false;

            _context.PriceLists.Remove(priceList);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivatePriceListAsync(int id)
        {
            var priceList = await _context.PriceLists.FindAsync(id);
            if (priceList == null) return false;

            priceList.IsActive = true;
            priceList.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivatePriceListAsync(int id)
        {
            var priceList = await _context.PriceLists.FindAsync(id);
            if (priceList == null) return false;

            priceList.IsActive = false;
            priceList.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // ==================== Listas Globales vs Específicas ====================

        // Obtener listas de precios disponibles para una tienda (globales + específicas)
        public async Task<List<PriceList>> GetPriceListsForStoreAsync(int storeId)
        {
            return await _context.PriceLists
                .Where(pl => pl.IsActive && (pl.StoreId == null || pl.StoreId == storeId))
                .Include(pl => pl.Store)
                .Include(pl => pl.PriceListItems)
                    .ThenInclude(pli => pli.Product)
                .Include(pl => pl.PriceListItems)
                    .ThenInclude(pli => pli.Discounts)
                .OrderBy(pl => pl.Name)
                .ToListAsync();
        }

        // Obtener solo listas específicas de una tienda
        public async Task<List<PriceList>> GetStoreSpecificPriceListsAsync(int storeId)
        {
            return await _context.PriceLists
                .Where(pl => pl.IsActive && pl.StoreId == storeId)
                .Include(pl => pl.Store)
                .Include(pl => pl.PriceListItems)
                    .ThenInclude(pli => pli.Product)
                .Include(pl => pl.PriceListItems)
                    .ThenInclude(pli => pli.Discounts)
                .OrderBy(pl => pl.Name)
                .ToListAsync();
        }

        // ==================== CRUD de PriceListItem ====================

        public async Task<PriceListItem?> GetPriceListItemByIdAsync(int id)
        {
            return await _context.PriceListItems
                .Include(pli => pli.Product)
                    .ThenInclude(p => p.ProductColors)
                .Include(pli => pli.PriceList)
                .Include(pli => pli.Discounts)
                .FirstOrDefaultAsync(pli => pli.Id == id);
        }

        public async Task<PriceListItem?> UpdatePriceListItemAsync(int id, PriceListItem updatedItem)
        {
            var item = await _context.PriceListItems.FindAsync(id);
            if (item == null) return null;

            item.Price = updatedItem.Price;
            item.Variant = updatedItem.Variant;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return item;
        }

        // ==================== Consultas de Precios ====================

        // Obtener el precio de un producto en una lista específica
        public async Task<PriceListItem?> GetProductPriceInListAsync(int priceListId, int productId)
        {
            return await _context.PriceListItems
                .Include(pli => pli.Product)
                .Include(pli => pli.PriceList)
                .Include(pli => pli.Discounts)
                .FirstOrDefaultAsync(pli => pli.PriceListId == priceListId && pli.ProductId == productId);
        }

        // Obtener el precio de un producto desde una lista de precios global (sin StoreId)
        // Retorna el PriceListItem con el precio según el PricingType del producto
        public async Task<PriceListItem?> GetProductPriceFromGlobalListAsync(int productId)
        {
            return await _context.PriceListItems
                .Include(pli => pli.Product)
                .Include(pli => pli.PriceList)
                .Include(pli => pli.Discounts)
                .Where(pli => pli.ProductId == productId && pli.PriceList.StoreId == null && pli.PriceList.IsActive)
                .OrderBy(pli => pli.PriceList.Name)
                .FirstOrDefaultAsync();
        }

        // Obtener el precio correcto de un producto según su PricingType desde una lista global
        public async Task<decimal?> GetProductPriceValueFromGlobalListAsync(int productId)
        {
            var item = await GetProductPriceFromGlobalListAsync(productId);

            if (item == null) return null;

            // Retornar el precio directamente (ya no depende del tipo)
            return item.Price;
        }

        // Obtener todos los productos con precios en una lista
        public async Task<List<PriceListItem>> GetProductsInPriceListAsync(int priceListId)
        {
            return await _context.PriceListItems
                .Where(pli => pli.PriceListId == priceListId)
                .Include(pli => pli.Product)
                    .ThenInclude(p => p.ProductColors)
                .Include(pli => pli.Discounts)
                .OrderBy(pli => pli.Product.Name)
                .ToListAsync();
        }

        // Obtener todas las listas que contienen un producto específico
        public async Task<List<PriceListItem>> GetPriceListsForProductAsync(int productId)
        {
            return await _context.PriceListItems
                .Where(pli => pli.ProductId == productId)
                .Include(pli => pli.PriceList)
                    .ThenInclude(pl => pl.Store)
                .Include(pli => pli.Discounts)
                .OrderBy(pli => pli.PriceList.Name)
                .ToListAsync();
        }

        // Obtener todos los precios de un producto agrupados por tienda
        public async Task<List<PriceListItem>> GetProductPricesByStoreAsync(int productId)
        {
            return await _context.PriceListItems
                .Where(pli => pli.ProductId == productId && pli.PriceList.IsActive)
                .Include(pli => pli.Product)
                .Include(pli => pli.PriceList)
                    .ThenInclude(pl => pl.Store)
                .Include(pli => pli.Discounts)
                .OrderBy(pli => pli.PriceList.StoreId.HasValue ? 1 : 0) // Globales primero
                .ThenBy(pli => pli.PriceList.Store.Name)
                .ThenBy(pli => pli.PriceList.Name)
                .ToListAsync();
        }

        // Obtener el precio según el tipo de producto
        public async Task<decimal?> GetProductPriceAsync(int priceListId, int productId)
        {
            var item = await _context.PriceListItems
                .Include(pli => pli.Product)
                .Include(pli => pli.Discounts)
                .FirstOrDefaultAsync(pli => pli.PriceListId == priceListId && pli.ProductId == productId);

            if (item == null) return null;

            // Retornar el precio final (con descuento si aplica)
            return item.GetFinalPrice();
        }

        // ==================== Operaciones en Lote ====================

        // Agregar múltiples productos a una lista con el mismo precio base
        public async Task<int> AddMultipleProductsAsync(int priceListId, List<int> productIds, decimal? basePrice = null)
        {
            var priceList = await _context.PriceLists.FindAsync(priceListId);
            if (priceList == null) return 0;

            int added = 0;
            foreach (var productId in productIds)
            {
                var exists = await _context.PriceListItems
                    .AnyAsync(pli => pli.PriceListId == priceListId && pli.ProductId == productId);

                if (!exists)
                {
                    var product = await _context.Products.FindAsync(productId);
                    if (product != null)
                    {
                        var item = new PriceListItem
                        {
                            PriceListId = priceListId,
                            ProductId = productId,
                            Price = basePrice ?? 0,
                            CreatedAt = DateTime.UtcNow
                        };

                        _context.PriceListItems.Add(item);
                        added++;
                    }
                }
            }

            if (added > 0)
            {
                await _context.SaveChangesAsync();
            }

            return added;
        }

        // Copiar todos los items de una lista a otra
        public async Task<int> CopyPriceListItemsAsync(int fromPriceListId, int toPriceListId)
        {
            var sourceItems = await _context.PriceListItems
                .Where(pli => pli.PriceListId == fromPriceListId)
                .ToListAsync();

            int copied = 0;
            foreach (var sourceItem in sourceItems)
            {
                var exists = await _context.PriceListItems
                    .AnyAsync(pli => pli.PriceListId == toPriceListId && pli.ProductId == sourceItem.ProductId);

                if (!exists)
                {
                    _context.PriceListItems.Add(new PriceListItem
                    {
                        PriceListId = toPriceListId,
                        ProductId = sourceItem.ProductId,
                        Price = sourceItem.Price,
                        Variant = sourceItem.Variant,
                        CreatedAt = DateTime.UtcNow
                    });
                    copied++;
                }
            }

            if (copied > 0)
            {
                await _context.SaveChangesAsync();
            }

            return copied;
        }

        // Aplicar un porcentaje de aumento/descuento a todos los items de una lista
        public async Task<int> ApplyPercentageToAllItemsAsync(int priceListId, decimal percentage)
        {
            var items = await _context.PriceListItems
                .Where(pli => pli.PriceListId == priceListId)
                .ToListAsync();

            foreach (var item in items)
            {
                var multiplier = 1 + (percentage / 100);
                item.Price = Math.Round(item.Price * multiplier, 2);
                item.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return items.Count;
        }

        // ==================== Validaciones ====================

        public async Task<bool> PriceListExistsAsync(int id)
        {
            return await _context.PriceLists.AnyAsync(pl => pl.Id == id);
        }

        public async Task<bool> IsPriceListNameUniqueAsync(string name, int? excludeId = null)
        {
            var query = _context.PriceLists.Where(pl => pl.Name == name);

            if (excludeId.HasValue)
            {
                query = query.Where(pl => pl.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }

        // ==================== Sincronización de ProductTypes ====================

        /// <summary>
        /// Sincroniza los productos de un ProductType a una PriceList.
        /// Agrega productos nuevos del ProductType que no existan en la lista.
        /// Elimina productos huérfanos (que ya no pertenecen al ProductType).
        /// </summary>
        public async Task<int> SyncProductsToPriceListAsync(int priceListId)
        {
            var priceList = await _context.PriceLists.FindAsync(priceListId);
            if (priceList?.ProductTypeId == null) return 0;

            // Productos que DEBERÍAN estar en la lista (activos del ProductType)
            var validProductIds = await _context.Products
                .Where(p => p.ProductTypeId == priceList.ProductTypeId && p.IsActive)
                .Select(p => p.Id)
                .ToListAsync();

            // 1. ELIMINAR productos huérfanos (que ya no pertenecen al ProductType o están inactivos)
            var orphanedItems = await _context.PriceListItems
                .Include(pli => pli.Product)
                .Where(pli => pli.PriceListId == priceListId
                           && pli.Variant == null // Solo productos base (no variantes)
                           && !validProductIds.Contains(pli.ProductId))
                .ToListAsync();

            if (orphanedItems.Any())
            {
                _context.PriceListItems.RemoveRange(orphanedItems);
            }

            // 2. AGREGAR productos nuevos que faltan
            int added = 0;
            foreach (var productId in validProductIds)
            {
                var exists = await _context.PriceListItems
                    .AnyAsync(pli => pli.PriceListId == priceListId
                                  && pli.ProductId == productId
                                  && pli.Variant == null);

                if (!exists)
                {
                    _context.PriceListItems.Add(new PriceListItem
                    {
                        PriceListId = priceListId,
                        ProductId = productId,
                        Price = 0,
                        Variant = null,
                        CreatedAt = DateTime.UtcNow
                    });
                    added++;
                }
            }

            if (added > 0 || orphanedItems.Any())
            {
                await _context.SaveChangesAsync();
            }

            return added + orphanedItems.Count; // Total de cambios realizados
        }

        /// <summary>
        /// Sincroniza un producto nuevo a todas las listas vinculadas a su ProductType.
        /// Se llama automáticamente cuando se crea o actualiza un producto.
        /// </summary>
        public async Task<int> SyncNewProductToLinkedPriceListsAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null || product.ProductTypeId == 0 || !product.IsActive) return 0;

            // Buscar todas las listas vinculadas a este ProductType
            var linkedLists = await _context.PriceLists
                .Where(pl => pl.ProductTypeId == product.ProductTypeId && pl.IsActive)
                .ToListAsync();

            int added = 0;
            foreach (var priceList in linkedLists)
            {
                // Verificar que no exista ya
                var exists = await _context.PriceListItems
                    .AnyAsync(pli => pli.PriceListId == priceList.Id && pli.ProductId == productId && pli.Variant == null);

                if (!exists)
                {
                    _context.PriceListItems.Add(new PriceListItem
                    {
                        PriceListId = priceList.Id,
                        ProductId = productId,
                        Price = 0,
                        Variant = null,
                        CreatedAt = DateTime.UtcNow
                    });
                    added++;
                }
            }

            if (added > 0)
            {
                await _context.SaveChangesAsync();
            }

            return added;
        }

        /// <summary>
        /// Obtiene el precio de un producto para una tienda específica.
        /// Si la tienda no tiene precio definido (o es 0), hereda del precio global.
        /// </summary>
        public async Task<decimal?> GetProductPriceForStoreAsync(int productId, int storeId, int productTypeId, string? variant = null)
        {
            // 1. Buscar precio específico de la tienda
            var storePriceList = await _context.PriceLists
                .FirstOrDefaultAsync(pl => pl.StoreId == storeId
                                        && pl.ProductTypeId == productTypeId
                                        && pl.IsActive);

            if (storePriceList != null)
            {
                var storePrice = await _context.PriceListItems
                    .Include(pli => pli.Discounts)
                    .Where(pli => pli.PriceListId == storePriceList.Id
                               && pli.ProductId == productId
                               && pli.Variant == variant)
                    .FirstOrDefaultAsync();

                // Si tiene precio específico Y no es 0, usarlo
                if (storePrice != null && storePrice.Price > 0)
                {
                    return storePrice.GetFinalPrice(); // Con descuentos si aplica
                }
            }

            // 2. Si no tiene precio específico, buscar precio global
            var globalPriceList = await _context.PriceLists
                .FirstOrDefaultAsync(pl => pl.StoreId == null
                                        && pl.ProductTypeId == productTypeId
                                        && pl.IsActive);

            if (globalPriceList != null)
            {
                var globalPrice = await _context.PriceListItems
                    .Include(pli => pli.Discounts)
                    .Where(pli => pli.PriceListId == globalPriceList.Id
                               && pli.ProductId == productId
                               && pli.Variant == variant)
                    .FirstOrDefaultAsync();

                if (globalPrice != null && globalPrice.Price > 0)
                {
                    return globalPrice.GetFinalPrice();
                }
            }

            // 3. No hay precio definido
            return null;
        }
    }
}
