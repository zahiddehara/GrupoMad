using GrupoMad.Data;
using GrupoMad.Models;
using Microsoft.EntityFrameworkCore;

namespace GrupoMad.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==================== CRUD de Productos ====================

        public async Task<List<Product>> GetAllProductsAsync(bool includeInactive = false)
        {
            var query = _context.Products
                .Include(p => p.Store)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
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
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product?> GetProductBySKUAsync(string sku)
        {
            return await _context.Products
                .Include(p => p.Store)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .FirstOrDefaultAsync(p => p.SKU == sku);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            product.CreatedAt = DateTime.UtcNow;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> UpdateProductAsync(int id, Product updatedProduct)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return null;

            product.SKU = updatedProduct.SKU;
            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.ProductType = updatedProduct.ProductType;
            product.PricingType = updatedProduct.PricingType;
            product.StoreId = updatedProduct.StoreId;
            product.IsActive = updatedProduct.IsActive;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
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

        public async Task<List<Color>> GetProductColorsAsync(int productId)
        {
            var product = await _context.Products
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .FirstOrDefaultAsync(p => p.Id == productId);

            return product?.ProductColors.Select(pc => pc.Color).ToList() ?? new List<Color>();
        }

        public async Task<bool> AddColorToProductAsync(int productId, int colorId, string sku)
        {
            var product = await _context.Products.FindAsync(productId);
            var color = await _context.Colors.FindAsync(colorId);

            if (product == null || color == null) return false;

            // Verificar si la relación ya existe
            var exists = await _context.ProductColors
                .AnyAsync(pc => pc.ProductId == productId && pc.ColorId == colorId);

            if (exists) return false;

            // Verificar que el SKU sea único
            var skuExists = await _context.ProductColors
                .AnyAsync(pc => pc.SKU == sku);

            if (skuExists) return false;

            _context.ProductColors.Add(new ProductColor
            {
                ProductId = productId,
                ColorId = colorId,
                SKU = sku
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddMultipleColorsToProductAsync(int productId, List<int> colorIds)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return false;

            foreach (var colorId in colorIds)
            {
                var exists = await _context.ProductColors
                    .AnyAsync(pc => pc.ProductId == productId && pc.ColorId == colorId);

                if (!exists)
                {
                    _context.ProductColors.Add(new ProductColor
                    {
                        ProductId = productId,
                        ColorId = colorId
                    });
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveColorFromProductAsync(int productId, int colorId)
        {
            var productColor = await _context.ProductColors
                .FirstOrDefaultAsync(pc => pc.ProductId == productId && pc.ColorId == colorId);

            if (productColor == null) return false;

            _context.ProductColors.Remove(productColor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProductColorSKUAsync(int productId, int colorId, string newSku)
        {
            var productColor = await _context.ProductColors
                .FirstOrDefaultAsync(pc => pc.ProductId == productId && pc.ColorId == colorId);

            if (productColor == null) return false;

            // Verificar que el nuevo SKU sea único
            var skuExists = await _context.ProductColors
                .AnyAsync(pc => pc.SKU == newSku && (pc.ProductId != productId || pc.ColorId != colorId));

            if (skuExists) return false;

            productColor.SKU = newSku;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ProductColor?> GetProductColorAsync(int productId, int colorId)
        {
            return await _context.ProductColors
                .Include(pc => pc.Product)
                .Include(pc => pc.Color)
                .FirstOrDefaultAsync(pc => pc.ProductId == productId && pc.ColorId == colorId);
        }

        // ==================== Búsqueda y Filtrado ====================

        public async Task<List<Product>> GetProductsByTypeAsync(ProductType productType)
        {
            return await _context.Products
                .Where(p => p.ProductType == productType && p.IsActive)
                .Include(p => p.Store)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsByColorAsync(int colorId)
        {
            return await _context.Products
                .Where(p => p.ProductColors.Any(pc => pc.ColorId == colorId) && p.IsActive)
                .Include(p => p.Store)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsByStoreAsync(int storeId)
        {
            return await _context.Products
                .Where(p => p.StoreId == storeId && p.IsActive)
                .Include(p => p.Store)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
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
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .ToListAsync();
        }

        // ==================== Productos Globales vs Específicos ====================

        // Obtener todos los productos visibles para una store (globales + específicos de la store)
        public async Task<List<Product>> GetProductsForStoreAsync(int storeId)
        {
            return await _context.Products
                .Where(p => p.IsActive && (p.StoreId == null || p.StoreId == storeId))
                .Include(p => p.Store)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        // Obtener solo productos globales (disponibles en todas las tiendas)
        public async Task<List<Product>> GetGlobalProductsAsync()
        {
            return await _context.Products
                .Where(p => p.IsActive && p.StoreId == null)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        // Obtener solo productos específicos de una store
        public async Task<List<Product>> GetStoreSpecificProductsAsync(int storeId)
        {
            return await _context.Products
                .Where(p => p.IsActive && p.StoreId == storeId)
                .Include(p => p.Store)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
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
