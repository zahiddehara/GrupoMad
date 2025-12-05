using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrupoMad.Models
{
    /// <summary>
    /// Rangos de precio por largo (1 dimensión)
    /// Ejemplo: Cortinas, persianas enrollables
    /// Se utiliza cuando el precio varía según el largo del producto
    /// </summary>
    public class PriceRangeByLength
    {
        public int Id { get; set; }

        [Required]
        public int PriceListItemId { get; set; }
        public PriceListItem PriceListItem { get; set; }

        /// <summary>
        /// Largo mínimo del rango (inclusive) en metros
        /// Por ejemplo: 1.00
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MinLength { get; set; }

        /// <summary>
        /// Largo máximo del rango (inclusive) en metros
        /// Por ejemplo: 2.50
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MaxLength { get; set; }

        /// <summary>
        /// Precio para este rango
        /// Por ejemplo: $7000.00
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Verifica si un valor de largo está dentro de este rango
        /// </summary>
        public bool IsInRange(decimal length)
        {
            return length >= MinLength && length <= MaxLength;
        }

        /// <summary>
        /// Retorna una representación legible del rango
        /// </summary>
        public string GetRangeDescription()
        {
            return $"{MinLength:F2}m a {MaxLength:F2}m: ${Price:N2}";
        }
    }
}
