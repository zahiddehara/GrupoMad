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

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        /// <summary>
        /// Variante del producto (ej: "3 vías", "4 vías", "5 vías").
        /// Null si el producto no tiene variantes.
        /// </summary>
        public string? Variant { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public List<PriceListItemDiscount>? Discounts { get; set; }

        // Métodos helper para cálculo de precios

        /// <summary>
        /// Obtiene el precio base
        /// </summary>
        public decimal GetBasePrice()
        {
            return Price;
        }

        /// <summary>
        /// Obtiene el precio final: busca el descuento vigente de mayor prioridad, si no hay retorna el precio base
        /// </summary>
        public decimal GetFinalPrice(DateTime? date = null)
        {
            var checkDate = date ?? DateTime.UtcNow;

            // Buscar descuentos vigentes ordenados por prioridad (menor número = mayor prioridad)
            var activeDiscount = Discounts?
                .Where(d => d.ValidFrom <= checkDate && d.ValidUntil >= checkDate)
                .OrderBy(d => d.Priority)
                .FirstOrDefault();

            // Si hay descuento vigente, usar ese; sino usar el precio base
            return activeDiscount?.DiscountedPrice ?? GetBasePrice();
        }

        /// <summary>
        /// Obtiene el monto del descuento aplicado (diferencia entre precio base y precio con descuento)
        /// </summary>
        public decimal GetDiscountApplied(DateTime? date = null)
        {
            var basePrice = GetBasePrice();
            var finalPrice = GetFinalPrice(date);

            // Solo hay descuento si el precio final es menor que el base
            if (finalPrice < basePrice)
            {
                return basePrice - finalPrice;
            }

            return 0;
        }

        /// <summary>
        /// Verifica si este item tiene un descuento aplicado
        /// </summary>
        public bool HasDiscount(DateTime? date = null)
        {
            return GetDiscountApplied(date) > 0;
        }

        /// <summary>
        /// Obtiene el descuento activo de mayor prioridad
        /// </summary>
        public PriceListItemDiscount? GetActiveDiscount(DateTime? date = null)
        {
            var checkDate = date ?? DateTime.UtcNow;

            return Discounts?
                .Where(d => d.ValidFrom <= checkDate && d.ValidUntil >= checkDate)
                .OrderBy(d => d.Priority)
                .FirstOrDefault();
        }
    }

    public class PriceListItemDiscount
    {
        public int Id { get; set; }

        [Required]
        public int PriceListItemId { get; set; }

        public PriceListItem? PriceListItem { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountedPrice { get; set; }

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidUntil { get; set; }

        /// <summary>
        /// Prioridad del descuento. Menor número = mayor prioridad (1 es la más alta)
        /// </summary>
        [Required]
        public int Priority { get; set; } = 1;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
