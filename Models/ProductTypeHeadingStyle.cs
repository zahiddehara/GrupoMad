using System.ComponentModel.DataAnnotations;

namespace GrupoMad.Models
{
    public class ProductTypeHeadingStyle
    {
        public int Id { get; set; }

        [Required]
        public int ProductTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public ProductType ProductType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
