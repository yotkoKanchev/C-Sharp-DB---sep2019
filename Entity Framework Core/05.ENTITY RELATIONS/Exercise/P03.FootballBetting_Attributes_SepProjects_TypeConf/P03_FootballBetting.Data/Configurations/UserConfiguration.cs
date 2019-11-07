namespace P03_FootballBetting.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Data.Models;
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {

            entity
                .Property(e => e.Username)
                .IsUnicode(false);

            entity
                .HasIndex(e => e.Username)
                .IsUnique();

            entity
                .Property(e => e.Password)
                .IsUnicode(false);

            entity
               .Property(e => e.Email)
               .IsUnicode(false);

            entity
                .HasIndex(e => e.Email)
                .IsUnique();
        }
    }
}
