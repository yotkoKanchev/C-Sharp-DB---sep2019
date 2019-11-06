namespace P03_FootballBetting.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P03_FootballBetting.Data.Models;
    using static DataValidations;

    public class TownConfiguration : IEntityTypeConfiguration<Town>
    {
        public void Configure(EntityTypeBuilder<Town> entity)
        {
            entity
                .HasKey(e => e.TownId);

            entity
                .Property(e => e.Name)
                .HasMaxLength(TownMaxNameLength)
                .IsRequired(true)
                .IsUnicode(true);

            entity
                .HasOne(t => t.Country)
                .WithMany(c => c.Towns)
                .HasForeignKey(t => t.CountryId)
                .HasConstraintName("FK_Towns_Countries")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
