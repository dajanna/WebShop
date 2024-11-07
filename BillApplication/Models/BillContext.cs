using BillApplication.Models;
using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;
using BillApplication.Models;

namespace BillApplication.Models
{
    public class BillContext : DbContext
    {
        public BillContext(DbContextOptions<BillContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Racun> Bills { get; set; }
        public virtual DbSet<Stavke> BillItems { get; set; }
        public virtual DbSet<Status> BillStatuses { get; set; }
        public virtual DbSet<Proizvod> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Konfiguracija za Racun
            modelBuilder.Entity<Racun>(entity =>
            {
                entity.ToTable("Bill");
                entity.HasKey(e => e.BillID);  // Primarni ključ
                entity.Property(e => e.BillID).HasColumnName("BillID");
                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Date).HasColumnType("datetime2");

                // Veza sa Status entitetom
                entity.HasOne(r => r.Status)
                      .WithMany(s => s.Bills)
                      .HasForeignKey(r => r.StatusId)
                      .OnDelete(DeleteBehavior.Restrict);  // Opcioni za brisanje
            });

            // Konfiguracija za Proizvod
            modelBuilder.Entity<Proizvod>(entity =>
            {
                entity.ToTable("Product");
                entity.HasKey(e => e.ProductID);  // Primarni ključ
                entity.Property(e => e.ProductID).HasColumnName("ProductID");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                // Veza sa Stavke entitetom
                entity.HasMany(p => p.Items)
                      .WithOne(s => s.Product)
                      .HasForeignKey(s => s.ProductID);
            });

            // Konfiguracija za Status
            modelBuilder.Entity<Status>(entity =>
            {
                entity.ToTable("Status");
                entity.HasKey(e => e.StatusID);  // Primarni ključ
                entity.Property(e => e.StatusID).HasColumnName("StatusID");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);

                // Veza sa Racun entitetom
                entity.HasMany(s => s.Bills)
                      .WithOne(r => r.Status)
                      .HasForeignKey(r => r.StatusId);
            });

            // Konfiguracija za Stavke
            modelBuilder.Entity<Stavke>(entity =>
            {
                entity.ToTable("BillItems");
                entity.HasKey(e => e.ItemsID);  // Primarni ključ
                entity.Property(e => e.ItemsID).HasColumnName("ItemsID");
                entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Kolicina).IsRequired();

                // Veza sa Racun i Proizvod entitetima
                entity.HasOne(s => s.Bill)
                      .WithMany(r => r.Items)
                      .HasForeignKey(s => s.BillID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.Product)
                      .WithMany(p => p.Items)
                      .HasForeignKey(s => s.ProductID)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
