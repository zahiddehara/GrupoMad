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
            modelBuilder.Entity<ProductColor>()
                .HasKey(pc => pc.Id);

            modelBuilder.Entity<ProductColor>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductColors)
                .HasForeignKey(pc => pc.ProductId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.ProductType)
                .WithMany(pt => pt.Products)
                .HasForeignKey(p => p.ProductTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductTypeVariant>()
                .HasOne(ptv => ptv.ProductType)
                .WithMany(pt => pt.ProductTypeVariants)
                .HasForeignKey(ptv => ptv.ProductTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configurar relación PriceListItem - ProductTypeVariant
            modelBuilder.Entity<PriceListItem>()
                .HasOne(pli => pli.ProductTypeVariant)
                .WithMany()
                .HasForeignKey(pli => pli.ProductTypeVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar relación ShippingAddress - Contact
            modelBuilder.Entity<ShippingAddress>()
                .HasOne(sa => sa.Contact)
                .WithMany(c => c.ShippingAddresses)
                .HasForeignKey(sa => sa.ContactID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar relación Contact - Company
            modelBuilder.Entity<Contact>()
                .HasOne(c => c.Company)
                .WithMany(co => co.Contacts)
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar relación User - Store
            modelBuilder.Entity<User>()
                .HasOne(u => u.Store)
                .WithMany(s => s.Users)
                .HasForeignKey(u => u.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índice único para Email de usuario
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configurar relación Quotation - Contact
            modelBuilder.Entity<Quotation>()
                .HasOne(q => q.Contact)
                .WithMany(c => c.Quotations)
                .HasForeignKey(q => q.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar relación Quotation - Store
            modelBuilder.Entity<Quotation>()
                .HasOne(q => q.Store)
                .WithMany(s => s.Quotations)
                .HasForeignKey(q => q.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar relación Quotation - User (CreatedByUser)
            modelBuilder.Entity<Quotation>()
                .HasOne(q => q.CreatedByUser)
                .WithMany(u => u.Quotations)
                .HasForeignKey(q => q.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar relación QuotationItem - Quotation
            modelBuilder.Entity<QuotationItem>()
                .HasOne(qi => qi.Quotation)
                .WithMany(q => q.Items)
                .HasForeignKey(qi => qi.QuotationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configurar relación QuotationItem - Product
            modelBuilder.Entity<QuotationItem>()
                .HasOne(qi => qi.Product)
                .WithMany()
                .HasForeignKey(qi => qi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<QuotationItem>()
                .HasOne(qi => qi.ProductTypeVariant)
                .WithMany()
                .HasForeignKey(qi => qi.ProductTypeVariantId)
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
        public DbSet<ProductTypeVariant> ProductTypeVariants { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<PriceList> PriceLists { get; set; }
        public DbSet<PriceListItem> PriceListItems { get; set; }
        public DbSet<PriceListItemDiscount> PriceListItemDiscounts { get; set; }
        public DbSet<PriceRangeByLength> PriceRangesByLength { get; set; }
        public DbSet<PriceRangeByDimensions> PriceRangesByDimensions { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ShippingAddress> ShippingAddresses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<QuotationItem> QuotationItems { get; set; }
    }
}
