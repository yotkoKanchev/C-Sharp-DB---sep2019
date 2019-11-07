namespace P03_FootballBetting.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Data.Models;

    public class PlayerStatisticConfiguration : IEntityTypeConfiguration<PlayerStatistic>
    {
        public void Configure(EntityTypeBuilder<PlayerStatistic> entity)
        {
            entity
                .HasKey(e => new { e.PlayerId, e.GameId });

            entity
                .ToTable("PlayerStatistics");

            entity
                .HasOne(ps => ps.Game)
                .WithMany(g => g.PlayerStatistics)
                .HasForeignKey(ps => ps.GameId)
                .HasForeignKey("FK_PlayerStatistics_Games")
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(ps => ps.Player)
                .WithMany(p => p.PlayerStatistics)
                .HasForeignKey(ps => ps.PlayerId)
                .HasConstraintName("FK_PlayerStatistics_Players")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
