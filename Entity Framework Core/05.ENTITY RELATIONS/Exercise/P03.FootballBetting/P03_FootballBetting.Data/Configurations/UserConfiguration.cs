namespace P03_FootballBetting.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P03_FootballBetting.Data.Models;
    using static DataValidations;
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity
                   .HasKey(e => e.UserId);

            entity
                .Property(e => e.Username)
                .HasMaxLength(UserMaxNameLength)
                .IsRequired(true)
                .IsUnicode(false);

            entity
                .HasIndex(e => e.Username)
                .IsUnique();

            entity
                .Property(e => e.Password)
                .HasMaxLength(PasswordMaxLength)
                .IsRequired(true)
                .IsUnicode(false);

            entity
               .Property(e => e.Email)
               .HasMaxLength(EmailMaxLength)
               .IsRequired(true)
               .IsUnicode(false);

            entity
                .HasIndex(e => e.Email)
                .IsUnique();

            entity
                .Property(e => e.Name)
                .HasMaxLength(UserMaxNameLength)
                .IsRequired(true)
                .IsUnicode(true);

            entity
                .Property(e => e.Balance)
                .IsRequired();
        }
    }
}
