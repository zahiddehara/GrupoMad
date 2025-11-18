using GrupoMad.Data;
using GrupoMad.Models;
using Microsoft.EntityFrameworkCore;

namespace GrupoMad.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly PriceListService _priceListService;

        public ProductService(ApplicationDbContext context, PriceListService priceListService)
        {
            _context = context;
            _priceListService = priceListService;
        }

        // ==================== CRUD de Productos ====================

        public async Task<List<Product>> GetAllProductsAsync(bool includeInactive = false)
        {
            var query = _context.Products
                .Include(p => p.Store)
                .Include(p => p.ProductType)
                .Include(p => p.ProductColors)
                .AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(p => p.IsActive);
            }

            return await query.ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Store)
                .Include(p => p.ProductType)
                .Include(p => p.ProductColors)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product?> GetProductBySKUAsync(string sku)
        {
            return await _context.Products
                .Include(p => p.Store)
                .Include(p => p.ProductType)
                .Include(p => p.ProductColors)
                .FirstOrDefaultAsync(p => p.SKU == sku);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            product.CreatedAt = DateTime.UtcNow;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Sincronizar con listas de precios vinculadas al ProductType
            if (product.ProductTypeId > 0)
            {
                await _priceListService.SyncNewProductToLinkedPriceListsAsync(product.Id);
            }

            return product;
        }

        public async Task<Product?> UpdateProductAsync(int id, Product updatedProduct)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return null;

            var oldProductTypeId = product.ProductTypeId;

            product.SKU = updatedProduct.SKU;
            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.ProductTypeId = updatedProduct.ProductTypeId;
            product.StoreId = updatedProduct.StoreId;
            product.IsActive = updatedProduct.IsActive;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Si cambió el ProductType y ahora tiene uno, sincronizar con las nuevas listas
            if (product.ProductTypeId > 0 && oldProductTypeId != product.ProductTypeId)
            {
                await _priceListService.SyncNewProductToLinkedPriceListsAsync(product.Id);
            }

            return product;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // ==================== Gestión de Colores ====================

        public async Task<List<ProductColor>> GetProductColorsAsync(int productId)
        {
            var product = await _context.Products
                .Include(p => p.ProductColors)
                .FirstOrDefaultAsync(p => p.Id == productId);

            return product?.ProductColors ?? new List<ProductColor>();
        }

        public async Task<bool> AddColorToProductAsync(int productId, string colorName, string sku)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null) return false;

            // Verificar que el SKU sea único
            var skuExists = await _context.ProductColors
                .AnyAsync(pc => pc.SKU == sku);

            if (skuExists) return false;

            _context.ProductColors.Add(new ProductColor
            {
                ProductId = productId,
                Name = colorName,
                SKU = sku,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddMultipleColorsToProductAsync(int productId, List<string> colorNames)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return false;

            foreach (var colorName in colorNames)
            {
                // Generar un SKU automático (puedes cambiar esta lógica según tus necesidades)
                var baseSku = $"{product.SKU}-{colorName.Replace(" ", "").ToUpper()}";
                var sku = baseSku;
                var counter = 1;

                // Asegurar que el SKU sea único
                while (await _context.ProductColors.AnyAsync(pc => pc.SKU == sku))
                {
                    sku = $"{baseSku}-{counter}";
                    counter++;
                }

                _context.ProductColors.Add(new ProductColor
                {
                    ProductId = productId,
                    Name = colorName,
                    SKU = sku,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveColorFromProductAsync(int productColorId)
        {
            var productColor = await _context.ProductColors
                .FirstOrDefaultAsync(pc => pc.Id == productColorId);

            if (productColor == null) return false;

            _context.ProductColors.Remove(productColor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProductColorAsync(int productColorId, string? newName, string? newSku)
        {
            var productColor = await _context.ProductColors
                .FirstOrDefaultAsync(pc => pc.Id == productColorId);

            if (productColor == null) return false;

            // Verificar que el nuevo SKU sea único si se proporciona
            if (!string.IsNullOrEmpty(newSku) && newSku != productColor.SKU)
            {
                var skuExists = await _context.ProductColors
                    .AnyAsync(pc => pc.SKU == newSku && pc.Id != productColorId);

                if (skuExists) return false;

                productColor.SKU = newSku;
            }

            // Actualizar nombre si se proporciona
            if (!string.IsNullOrEmpty(newName))
            {
                productColor.Name = newName;
            }

            productColor.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ProductColor?> GetProductColorAsync(int productColorId)
        {
            return await _context.ProductColors
                .Include(pc => pc.Product)
                .FirstOrDefaultAsync(pc => pc.Id == productColorId);
        }

        // ==================== Búsqueda y Filtrado ====================

        public async Task<List<Product>> GetProductsByTypeAsync(int productTypeId)
        {
            return await _context.Products
                .Where(p => p.ProductTypeId == productTypeId && p.IsActive)
                .Include(p => p.Store)
                .Include(p => p.ProductType)
                .Include(p => p.ProductColors)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsByColorNameAsync(string colorName)
        {
            return await _context.Products
                .Where(p => p.ProductColors.Any(pc => pc.Name.Contains(colorName)) && p.IsActive)
                .Include(p => p.Store)
                .Include(p => p.ProductType)
                .Include(p => p.ProductColors)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsByStoreAsync(int storeId)
        {
            return await _context.Products
                .Where(p => p.StoreId == storeId && p.IsActive)
                .Include(p => p.Store)
                .Include(p => p.ProductType)
                .Include(p => p.ProductColors)
                .ToListAsync();
        }

        public async Task<List<Product>> SearchProductsAsync(string searchTerm)
        {
            return await _context.Products
                .Where(p => (p.Name.Contains(searchTerm) ||
                            p.SKU.Contains(searchTerm) ||
                            (p.Description != null && p.Description.Contains(searchTerm))) &&
                            p.IsActive)
                .Include(p => p.Store)
                .Include(p => p.ProductType)
                .Include(p => p.ProductColors)
                .ToListAsync();
        }

        // ==================== Productos Globales vs Específicos ====================

        // Obtener todos los productos visibles para una store (globales + específicos de la store)
        public async Task<List<Product>> GetProductsForStoreAsync(int storeId)
        {
            return await _context.Products
                .Where(p => p.IsActive && (p.StoreId == null || p.StoreId == storeId))
                .Include(p => p.Store)
                .Include(p => p.ProductType)
                .Include(p => p.ProductColors)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        // Obtener solo productos globales (disponibles en todas las tiendas)
        public async Task<List<Product>> GetGlobalProductsAsync()
        {
            return await _context.Products
                .Where(p => p.IsActive && p.StoreId == null)
                .Include(p => p.ProductType)
                .Include(p => p.ProductColors)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        // Obtener solo productos específicos de una store
        public async Task<List<Product>> GetStoreSpecificProductsAsync(int storeId)
        {
            return await _context.Products
                .Where(p => p.IsActive && p.StoreId == storeId)
                .Include(p => p.Store)
                .Include(p => p.ProductType)
                .Include(p => p.ProductColors)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        // Verificar si un producto está disponible en una tienda específica
        public async Task<bool> IsProductAvailableInStoreAsync(int productId, int storeId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null || !product.IsActive) return false;

            return product.StoreId == null || product.StoreId == storeId;
        }

        // ==================== Validaciones ====================

        public async Task<bool> IsSKUUniqueAsync(string sku, int? excludeProductId = null)
        {
            var query = _context.Products.Where(p => p.SKU == sku);

            if (excludeProductId.HasValue)
            {
                query = query.Where(p => p.Id != excludeProductId.Value);
            }

            return !await query.AnyAsync();
        }
    }
}
