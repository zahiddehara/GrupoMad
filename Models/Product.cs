using System.ComponentModel.DataAnnotations;

namespace GrupoMad.Models
{
    public enum PricingType
    {
        PerSquareMeter,      // Por metro cuadrado (ancho * alto)
        PerUnit,             // Por unidad
        PerLinearMeter,      // Por metro lineal (ancho)
        PerRangeLength,      // Por rango de largo (1D)
        PerRangeDimensions   // Por rango de dimensiones (2D: ancho y alto)
    }

    public class ProductType
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public PricingType PricingType { get; set; }

        public bool HasVariants { get; set; } = false;

        public bool HasHeadingStyles { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public List<Product> Products { get; set; } = new List<Product>();

        public List<ProductTypeVariant> ProductTypeVariants { get; set; } = new List<ProductTypeVariant>();

        public List<ProductTypeHeadingStyle> ProductTypeHeadingStyles { get; set; } = new List<ProductTypeHeadingStyle>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string SKU { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public int ProductTypeId { get; set; }

        public ProductType ProductType { get; set; }

        public int? StoreId { get; set; }

        public Store Store { get; set; }

        public bool IsActive { get; set; } = true;

        public List<ProductColor> ProductColors { get; set; } = new List<ProductColor>();

        public List<PriceListItem> PriceListItems { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }

    public class ProductColor
    {
        public int Id { get; set; }

        [Required]
        public string SKU { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
