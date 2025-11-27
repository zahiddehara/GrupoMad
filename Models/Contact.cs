using System.ComponentModel.DataAnnotations;

namespace GrupoMad.Models
{
    public enum MexicanState
    {
        Aguascalientes = 1,
        BajaCalifornia,
        BajaCaliforniaSur,
        Campeche,
        Chiapas,
        Chihuahua,
        CiudadDeMexico,
        Coahuila,
        Colima,
        Durango,
        EstadoDeMexico,
        Guanajuato,
        Guerrero,
        Hidalgo,
        Jalisco,
        Michoacan,
        Morelos,
        Nayarit,
        NuevoLeon,
        Oaxaca,
        Puebla,
        Queretaro,
        QuintanaRoo,
        SanLuisPotosi,
        Sinaloa,
        Sonora,
        Tabasco,
        Tamaulipas,
        Tlaxcala,
        Veracruz,
        Yucatan,
        Zacatecas
    }

    public class Contact
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [StringLength(200)]
        public string Street { get; set; }

        [Required]
        [StringLength(20)]
        public string ExteriorNumber { get; set; }

        [StringLength(20)]
        public string? InteriorNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string Neighborhood { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        public MexicanState StateID { get; set; }

        [Required]
        [StringLength(10)]
        public string PostalCode { get; set; }

        [StringLength(13)]
        public string? RFC { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public Company Company { get; set; }

        public List<ShippingAddress> ShippingAddresses { get; set; } = new List<ShippingAddress>();

        public List<Quotation> Quotations { get; set; } = new List<Quotation>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
