namespace P03_FootballBetting.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P03_FootballBetting.Data.Models;

    public class GameConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> entity)
        {
            entity
                .HasKey(e => e.GameId);

            entity
                .Property(e => e.HomeTeamGoals)
                .IsRequired();

            entity
                .Property(e => e.AwayTeamGoals)
                .IsRequired();

            entity
                .Property(e => e.DateTime)
                .IsRequired();

            entity
                .Property(e => e.HomeTeamBetRate)
                .IsRequired();

            entity
                .Property(e => e.DrawBetRate)
                .IsRequired();

            entity
                .Property(e => e.AwayTeamBetRate)
                .IsRequired();

            entity
                .Property(e => e.Result)
                .IsRequired();

            entity
                .HasOne(g => g.HomeTeam)
                .WithMany(ht => ht.HomeGames)
                .HasForeignKey(g => g.HomeTeamId)
                .HasConstraintName("FK_Games_Teams_Home")
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(g => g.AwayTeam)
                .WithMany(at => at.AwayGames)
                .HasForeignKey(g => g.AwayTeamId)
                .HasConstraintName("FK_Games_Teams_Away")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
