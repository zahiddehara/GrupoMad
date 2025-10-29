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
                .Include(pl => pl.PriceListItems)
                    .ThenInclude(pli => pli.Product)
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
                .Include(pl => pl.PriceListItems)
                    .ThenInclude(pli => pli.Product)
                        .ThenInclude(p => p.ProductColors)
                            .ThenInclude(pc => pc.Color)
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
                .OrderBy(pl => pl.Name)
                .ToListAsync();
        }

        // Obtener solo listas de precios globales
        public async Task<List<PriceList>> GetGlobalPriceListsAsync()
        {
            return await _context.PriceLists
                .Where(pl => pl.IsActive && pl.StoreId == null)
                .Include(pl => pl.PriceListItems)
                    .ThenInclude(pli => pli.Product)
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
                .OrderBy(pl => pl.Name)
                .ToListAsync();
        }

        // ==================== CRUD de PriceListItem ====================

        public async Task<PriceListItem?> GetPriceListItemByIdAsync(int id)
        {
            return await _context.PriceListItems
                .Include(pli => pli.Product)
                    .ThenInclude(p => p.ProductColors)
                        .ThenInclude(pc => pc.Color)
                .Include(pli => pli.PriceList)
                .FirstOrDefaultAsync(pli => pli.Id == id);
        }

        public async Task<PriceListItem> AddItemToPriceListAsync(PriceListItem item)
        {
            // Verificar que no exista ya el producto en la lista
            var exists = await _context.PriceListItems
                .AnyAsync(pli => pli.PriceListId == item.PriceListId && pli.ProductId == item.ProductId);

            if (exists)
            {
                throw new InvalidOperationException("El producto ya existe en esta lista de precios.");
            }

            item.CreatedAt = DateTime.UtcNow;
            _context.PriceListItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<PriceListItem?> UpdatePriceListItemAsync(int id, PriceListItem updatedItem)
        {
            var item = await _context.PriceListItems.FindAsync(id);
            if (item == null) return null;

            item.PricePerSquareMeter = updatedItem.PricePerSquareMeter;
            item.PricePerUnit = updatedItem.PricePerUnit;
            item.PricePerLinearMeter = updatedItem.PricePerLinearMeter;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<bool> RemoveItemFromPriceListAsync(int itemId)
        {
            var item = await _context.PriceListItems.FindAsync(itemId);
            if (item == null) return false;

            _context.PriceListItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        // ==================== Consultas de Precios ====================

        // Obtener el precio de un producto en una lista específica
        public async Task<PriceListItem?> GetProductPriceInListAsync(int priceListId, int productId)
        {
            return await _context.PriceListItems
                .Include(pli => pli.Product)
                .Include(pli => pli.PriceList)
                .FirstOrDefaultAsync(pli => pli.PriceListId == priceListId && pli.ProductId == productId);
        }

        // Obtener todos los productos con precios en una lista
        public async Task<List<PriceListItem>> GetProductsInPriceListAsync(int priceListId)
        {
            return await _context.PriceListItems
                .Where(pli => pli.PriceListId == priceListId)
                .Include(pli => pli.Product)
                    .ThenInclude(p => p.ProductColors)
                        .ThenInclude(pc => pc.Color)
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
                .OrderBy(pli => pli.PriceList.Name)
                .ToListAsync();
        }

        // Obtener el precio según el tipo de producto
        public async Task<decimal?> GetProductPriceAsync(int priceListId, int productId)
        {
            var item = await _context.PriceListItems
                .Include(pli => pli.Product)
                .FirstOrDefaultAsync(pli => pli.PriceListId == priceListId && pli.ProductId == productId);

            if (item == null) return null;

            // Retornar el precio según el tipo de producto
            return item.Product.PricingType switch
            {
                PricingType.PerSquareMeter => item.PricePerSquareMeter,
                PricingType.PerUnit => item.PricePerUnit,
                PricingType.PerLinearMeter => item.PricePerLinearMeter,
                PricingType.PerRange => item.PricePerSquareMeter, // O la lógica que necesites para rangos
                _ => null
            };
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
                            CreatedAt = DateTime.UtcNow
                        };

                        // Asignar precio base según el tipo de producto
                        if (basePrice.HasValue)
                        {
                            switch (product.PricingType)
                            {
                                case PricingType.PerSquareMeter:
                                    item.PricePerSquareMeter = basePrice;
                                    break;
                                case PricingType.PerUnit:
                                    item.PricePerUnit = basePrice;
                                    break;
                                case PricingType.PerLinearMeter:
                                    item.PricePerLinearMeter = basePrice;
                                    break;
                            }
                        }

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
                        PricePerSquareMeter = sourceItem.PricePerSquareMeter,
                        PricePerUnit = sourceItem.PricePerUnit,
                        PricePerLinearMeter = sourceItem.PricePerLinearMeter,
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

                if (item.PricePerSquareMeter.HasValue)
                    item.PricePerSquareMeter = Math.Round(item.PricePerSquareMeter.Value * multiplier, 2);

                if (item.PricePerUnit.HasValue)
                    item.PricePerUnit = Math.Round(item.PricePerUnit.Value * multiplier, 2);

                if (item.PricePerLinearMeter.HasValue)
                    item.PricePerLinearMeter = Math.Round(item.PricePerLinearMeter.Value * multiplier, 2);

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
    }
}
