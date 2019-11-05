namespace P03_SalesDatabase.Data
{
    using Microsoft.EntityFrameworkCore;
    using P03_SalesDatabase.Data.Models;

    public class SalesConext : DbContext
    {
        public DbSet<Sale> Sales { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Store> Stores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DataSettings.DefaultConnection);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(s => s.SaleId);

                //entity.ToTable("Sales");

                entity.Property(s => s.Date)
                      .IsRequired(true)
					  .HasColumnType("DATETIME2");

                entity.HasOne(s => s.Product)
                      .WithMany(p => p.Sales)
                      .HasForeignKey(s => s.ProductId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_Sales_Products");

                entity.HasOne(s => s.Customer)
                      .WithMany(c => c.Sales)
                      .HasForeignKey(s => s.CustomerId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_Sales_Customers");

                entity.HasOne(s => s.Store)
                      .WithMany(st => st.Sales)
                      .HasForeignKey(s => s.StoreId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_Sales_Stores");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.ProductId);

                //entity.ToTable("Products");

                entity.Property(p => p.Name)
                      .HasMaxLength(50)
                      .IsRequired(true)
                      .IsUnicode(true);

                entity.Property(p => p.Quantity)
                      .IsRequired(true);

                entity.Property(p => p.Price)
                      .IsRequired(true);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.CustomerId);

                //entity.ToTable("Customers");

                entity.Property(c => c.Name)
                      .HasMaxLength(100)
                      .IsRequired(true)
                      .IsUnicode(true);

                entity.Property(c => c.Email)
                      .HasMaxLength(80)
                      .IsRequired(false)
                      .IsUnicode(false);

                entity.Property(c => c.CreditCardNumber)
                      .HasMaxLength(20)
                      .IsRequired(false)
                      .IsUnicode(false);
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.HasKey(s => s.StoreId);

                //entity.ToTable("Stores");

                entity.Property(s => s.Name)
                      .HasMaxLength(80)
                      .IsRequired(true)
                      .IsUnicode(true);
            });
        }
    }
}
