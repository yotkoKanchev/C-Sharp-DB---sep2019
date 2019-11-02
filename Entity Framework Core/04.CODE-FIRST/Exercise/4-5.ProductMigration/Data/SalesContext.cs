namespace P03_SalesDatabase.Data
{
    using Microsoft.EntityFrameworkCore;
    using Models;
    public class SalesContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Store> Stores { get; set; }

        public DbSet<Sale> Sales { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DataSettings.DafaultConnection);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity
                    .HasKey(e => e.ProductId);

                entity
                    .Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsRequired(true)
                    .IsUnicode(true);

                entity
                    .Property(e => e.Quantity)
                    .IsRequired(true);

                entity
                    .Property(e => e.Price)
                    .IsRequired(true);

                entity
                    .Property(e => e.Description)
                    .HasMaxLength(250)
                    .IsRequired(false)
                    .IsUnicode(true)
                    .HasDefaultValue("No description");

            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity
                    .HasKey(e => e.CustomerId);

                entity
                    .Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsRequired(true)
                    .IsUnicode(true);

                entity
                    .Property(e => e.Email)
                    .HasMaxLength(80)
                    .IsRequired(false)
                    .IsUnicode(false);

                entity
                    .Property(e => e.CreditCardNumber)
                    .HasMaxLength(16)
                    .IsRequired(true)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity
                    .HasKey(e => e.StoreId);

                entity
                    .Property(e => e.Name)
                    .HasMaxLength(80)
                    .IsRequired(true)
                    .IsUnicode(true);
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity
                    .HasKey(e => e.SaleId);

                entity
                    .Property(e => e.Date)
                    .IsRequired(true)
                    .HasColumnType("DATETIME2")
                    .HasDefaultValueSql("GETDATE()");

                entity
                     .HasOne(p => p.Product)
                     .WithMany(s => s.Sales)
                     .HasForeignKey(p => p.ProductId);

                entity
                     .HasOne(c => c.Customer)
                     .WithMany(s => s.Sales)
                     .HasForeignKey(c => c.CustomerId);

                entity
                     .HasOne(s => s.Store)
                     .WithMany(st => st.Sales)
                     .HasForeignKey(s => s.ProductId);
            });
        }
    }
}

