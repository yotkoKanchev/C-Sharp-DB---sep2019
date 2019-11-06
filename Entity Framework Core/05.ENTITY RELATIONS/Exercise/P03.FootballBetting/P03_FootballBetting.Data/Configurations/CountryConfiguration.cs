namespace P03_FootballBetting.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P03_FootballBetting.Data.Models;
    using static DataValidations;

    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> entity)
        {
            entity
                    .HasKey(e => e.CountryId);

            entity
                .ToTable("Countries");

            entity
                .Property(e => e.Name)
                .HasMaxLength(CountryMaxNameLength)
                .IsRequired(true)
                .IsUnicode(true);
        }
    }
}
