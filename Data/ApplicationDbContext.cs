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
            // Configurar ProductColor con clave primaria Id
            modelBuilder.Entity<ProductColor>()
                .HasKey(pc => pc.Id);

            modelBuilder.Entity<ProductColor>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductColors)
                .HasForeignKey(pc => pc.ProductId);

            // Configurar relación Product - ProductType
            modelBuilder.Entity<Product>()
                .HasOne(p => p.ProductType)
                .WithMany(pt => pt.Products)
                .HasForeignKey(p => p.ProductTypeId)
                .OnDelete(DeleteBehavior.Restrict);

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
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<PriceList> PriceLists { get; set; }
        public DbSet<PriceListItem> PriceListItems { get; set; }
        public DbSet<PriceListItemDiscount> PriceListItemDiscounts { get; set; }
    }
}
