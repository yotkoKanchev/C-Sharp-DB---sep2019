namespace P03_FootballBetting.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P03_FootballBetting.Data.Models;
    using static DataValidations;

    public class PositionConfiguration : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> entity)
        {
            entity
                 .HasKey(e => e.PositionId);

            entity
                .Property(e => e.Name)
                .HasMaxLength(PositionMaxNameLength)
                .IsRequired(true)
                .IsUnicode(true);
        }
    }
}
