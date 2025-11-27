using System.ComponentModel.DataAnnotations;

namespace GrupoMad.Models
{
    public enum UserRole
    {
        Administrator = 1,
        SalesManager = 2,
        StoreManager = 3,
        Salesperson = 4
    }

    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [Required]
        public UserRole Role { get; set; }

        // StoreId es nullable - null para Administrator que tiene acceso a todas las tiendas
        public int? StoreId { get; set; }

        public Store? Store { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? LastLoginAt { get; set; }
    }
}
