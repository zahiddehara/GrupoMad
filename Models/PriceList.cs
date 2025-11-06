using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrupoMad.Models
{
    public class PriceList
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int? StoreId { get; set; }

        public Store? Store { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public List<PriceListItem>? PriceListItems { get; set; }
    }

    public class PriceListItem
    {
        public int Id { get; set; }

        [Required]
        public int PriceListId { get; set; }

        public PriceList? PriceList { get; set; }

        [Required]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PricePerSquareMeter { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PricePerUnit { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PricePerLinearMeter { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountedPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Métodos helper para cálculo de precios

        /// <summary>
        /// Obtiene el precio base según el tipo de pricing del producto
        /// </summary>
        public decimal? GetBasePrice()
        {
            if (Product == null) return null;

            return Product.PricingType switch
            {
                PricingType.PerUnit => PricePerUnit,
                PricingType.PerSquareMeter => PricePerSquareMeter,
                PricingType.PerLinearMeter => PricePerLinearMeter,
                PricingType.PerRange => PricePerSquareMeter, // O la lógica que necesites para rangos
                _ => null
            };
        }

        /// <summary>
        /// Obtiene el precio final: si hay precio con descuento, lo retorna; sino retorna el precio base
        /// </summary>
        public decimal? GetFinalPrice()
        {
            // Si hay precio con descuento definido, usar ese; sino usar el precio base
            return DiscountedPrice ?? GetBasePrice();
        }

        /// <summary>
        /// Obtiene el monto del descuento aplicado (diferencia entre precio base y precio con descuento)
        /// </summary>
        public decimal GetDiscountApplied()
        {
            var basePrice = GetBasePrice();

            // Solo hay descuento si ambos precios existen y el precio con descuento es menor
            if (basePrice.HasValue && DiscountedPrice.HasValue && DiscountedPrice.Value < basePrice.Value)
            {
                return basePrice.Value - DiscountedPrice.Value;
            }

            return 0;
        }

        /// <summary>
        /// Verifica si este item tiene un descuento aplicado
        /// </summary>
        public bool HasDiscount()
        {
            return GetDiscountApplied() > 0;
        }
    }
}
