using System.ComponentModel.DataAnnotations;

namespace GrupoMad.Models
{
    public enum ProductType
    {
        Accessory,
        // Curtain,
        // Awning,
        // CurtainRod,
        // Panel,
        // Wave
        Blind
    }

    public enum PricingType
    {
        PerSquareMeter,
        PerUnit,
        PerRange,
        PerLinearMeter
    }

    public abstract class Product
    {
        public int Id { get; set; }

        [Required]
        public string SKU { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public ProductType ProductType { get; set; }

        [Required]
        public PricingType PricingType { get; set; }

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
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int ColorId { get; set; }
        public Color Color { get; set; }
    }

    public class AccessoryProduct : Product
    {
        public AccessoryProduct()
        {
            ProductType = ProductType.Accessory;
            PricingType = PricingType.PerUnit;
        }
    }

    public class BlindProduct : Product
    {
        public BlindProduct()
        {
            ProductType = ProductType.Blind;
            PricingType = PricingType.PerSquareMeter;
        }
    }
}
