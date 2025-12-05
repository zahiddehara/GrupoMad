using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrupoMad.Models
{
    /// <summary>
    /// Rangos de precio por ancho y alto (2 dimensiones)
    /// Ejemplo: Ventanas, puertas, persianas de aluminio
    /// Se utiliza cuando el precio varía según el ancho Y alto del producto
    /// </summary>
    public class PriceRangeByDimensions
    {
        public int Id { get; set; }

        [Required]
        public int PriceListItemId { get; set; }
        public PriceListItem PriceListItem { get; set; }

        /// <summary>
        /// Ancho mínimo del rango (inclusive) en metros
        /// Por ejemplo: 1.00
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MinWidth { get; set; }

        /// <summary>
        /// Ancho máximo del rango (inclusive) en metros
        /// Por ejemplo: 1.39
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MaxWidth { get; set; }

        /// <summary>
        /// Alto mínimo del rango (inclusive) en metros
        /// Por ejemplo: 1.00
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MinHeight { get; set; }

        /// <summary>
        /// Alto máximo del rango (inclusive) en metros
        /// Por ejemplo: 1.09
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MaxHeight { get; set; }

        /// <summary>
        /// Precio para este rango
        /// Por ejemplo: $285.00
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Verifica si los valores de ancho y alto están dentro de este rango
        /// </summary>
        public bool IsInRange(decimal width, decimal height)
        {
            return width >= MinWidth && width <= MaxWidth &&
                   height >= MinHeight && height <= MaxHeight;
        }

        /// <summary>
        /// Retorna una representación legible del rango
        /// </summary>
        public string GetRangeDescription()
        {
            return $"Ancho: {MinWidth:F2}m a {MaxWidth:F2}m | Alto: {MinHeight:F2}m a {MaxHeight:F2}m: ${Price:N2}";
        }
    }
}
