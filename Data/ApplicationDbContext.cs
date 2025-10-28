using GrupoMad.Models;
using Microsoft.EntityFrameworkCore;

namespace GrupoMad.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar herencia TPH con discriminador para Product
            modelBuilder.Entity<Product>()
                .HasDiscriminator<ProductType>("ProductType")
                .HasValue<AccessoryProduct>(ProductType.Accessory)
                .HasValue<BlindProduct>(ProductType.Blind);

            // Configurar la relación muchos-a-muchos entre Product y Color
            modelBuilder.Entity<ProductColor>()
                .HasKey(pc => new { pc.ProductId, pc.ColorId });

            modelBuilder.Entity<ProductColor>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductColors)
                .HasForeignKey(pc => pc.ProductId);

            modelBuilder.Entity<ProductColor>()
                .HasOne(pc => pc.Color)
                .WithMany(c => c.ProductColors)
                .HasForeignKey(pc => pc.ColorId);

            modelBuilder.Entity<Company>().HasData(
                new Company { Id = 1, Name = "Deconolux" },
                new Company { Id = 2, Name = "Persianas Mad" },
                new Company { Id = 3, Name = "Persitex" }
            );

            modelBuilder.Entity<Store>().HasData(
                new Store { Id = 1, Name = "Sucursal Pachuca Revolucion", CompanyId = 1 },
                new Store { Id = 2, Name = "Sucursal Pachuca Plaza de las Americas", CompanyId = 1 },
                new Store { Id = 3, Name = "Sucursal Puebla", CompanyId = 1 },
                new Store { Id = 4, Name = "Sucursal Tlaxcala", CompanyId = 1 },
                new Store { Id = 5, Name = "Sucursal Apizaco", CompanyId = 1 }
            );

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<PriceList> PriceLists { get; set; }
        public DbSet<PriceListItem> PriceListItems { get; set; }

        public DbSet<AccessoryProduct> Accessories { get; set; }
        public DbSet<BlindProduct> Blinds { get; set; }
    }
}
