using System.ComponentModel.DataAnnotations;

namespace GrupoMad.Models
{
    public class Color
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsActive { get; set; } = true;

        public List<ProductColor> ProductColors { get; set; } = new List<ProductColor>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
