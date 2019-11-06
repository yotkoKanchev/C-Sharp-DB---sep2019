namespace P03_FootballBetting.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P03_FootballBetting.Data.Models;
    using static DataValidations;

    public class PlayerConfiguration : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> entity)
        {
            entity
                 .HasKey(e => e.PlayerId);

            entity
                .Property(e => e.Name)
                .HasMaxLength(PlayerMaxNameLength)
                .IsRequired(true)
                .IsUnicode(true);

            entity
                .Property(e => e.SquadNumber)
                .IsRequired(true);

            entity
                .Property(e => e.IsInjured)
                .IsRequired(true)
                .HasDefaultValue(false);

            entity
                .HasOne(p => p.Team)
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TeamId)
                .HasConstraintName("FK_Players_Teams")
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(p => p.Position)
                .WithMany(po => po.Players)
                .HasForeignKey(p => p.PositionId)
                .HasConstraintName("FK_Players_Positions")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
