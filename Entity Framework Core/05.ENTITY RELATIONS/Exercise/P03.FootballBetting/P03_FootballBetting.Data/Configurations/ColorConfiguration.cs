namespace P03_FootballBetting.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P03_FootballBetting.Data.Models;
    using static DataValidations;

    public class ColorConfiguration : IEntityTypeConfiguration<Color>
    {
        public void Configure(EntityTypeBuilder<Color> entity)
        {
            entity
                 .HasKey(e => e.ColorId);

            entity
                .Property(e => e.Name)
                .HasMaxLength(ColorMaxNameLength)
                .IsRequired(true)
                .IsUnicode(true);
        }
    }
}
