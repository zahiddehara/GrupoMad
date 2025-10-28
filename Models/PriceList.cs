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

        public Store Store { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public List<PriceListItem> PriceListItems { get; set; }
    }

    public class PriceListItem
    {
        public int Id { get; set; }

        [Required]
        public int PriceListId { get; set; }

        public PriceList PriceList { get; set; }

        [Required]
        public int ProductId { get; set; }

        public Product Product { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PricePerSquareMeter { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PricePerUnit { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PricePerLinearMeter { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
