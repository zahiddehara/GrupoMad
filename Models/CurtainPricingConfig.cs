using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrupoMad.Models
{
    public class CurtainPricingConfig
    {
        public int Id { get; set; }

        [Required]
        public int PriceListItemId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal TaxPercent { get; set; }

        [Required]
        public CurtainPricingType PricingType { get; set; } = CurtainPricingType.Normal;

        /// <summary>
        /// JSON with profit margin percentages by height index.
        /// Format: {"0":60,"1":62,"2":64,...}
        /// The key is the index in DimensionRanges.LengthRanges (45 ranges) for Normal
        /// or DimensionRanges.SpecialLengthRanges (6 ranges) for Special
        /// </summary>
        [Required]
        public string ProfitMarginsByHeightJson { get; set; }

        // Navigation properties
        public PriceListItem PriceListItem { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
