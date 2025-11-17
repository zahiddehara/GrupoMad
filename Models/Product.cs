using System.ComponentModel.DataAnnotations;

namespace GrupoMad.Models
{
    public enum PricingType
    {
        PerSquareMeter,
        PerUnit,
        PerRange,
        PerLinearMeter
    }

    public class ProductType
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public PricingType PricingType { get; set; }

        public bool IsActive { get; set; } = true;

        public List<Product> Products { get; set; } = new List<Product>();

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
