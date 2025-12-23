using System.ComponentModel.DataAnnotations;

namespace GrupoMad.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        [StringLength(200)]
        public string? Street { get; set; }

        [StringLength(20)]
        public string? ExteriorNumber { get; set; }

        [StringLength(20)]
        public string? InteriorNumber { get; set; }

        [StringLength(100)]
        public string? Neighborhood { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        public MexicanState? StateID { get; set; }

        [StringLength(10)]
        public string? PostalCode { get; set; }

        [StringLength(13)]
        public string? RFC { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        public List<Store>? Stores { get; set; }
        public List<Contact> Contacts { get; set; } = new List<Contact>();
    }
}
