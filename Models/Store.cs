using System.ComponentModel.DataAnnotations.Schema;

namespace GrupoMad.Models
{
    public class Store
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }
    }
}
