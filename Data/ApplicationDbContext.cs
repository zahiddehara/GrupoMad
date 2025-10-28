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
    }
}
