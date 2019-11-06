namespace P03_FootballBetting.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P03_FootballBetting.Data.Models;
    public class BetConfiguration : IEntityTypeConfiguration<Bet>
    {
        public void Configure(EntityTypeBuilder<Bet> entity)
        {
            entity
                .HasKey(e => e.BetId);

            entity
                .Property(e => e.Amount)
                .IsRequired();

            entity
                .Property(e => e.Prediction)
                .IsRequired();

            entity
                .Property(e => e.DateTime)
                .IsRequired();

            entity
                .HasOne(b => b.User)
                .WithMany(u => u.Bets)
                .HasForeignKey(b => b.UserId)
                .HasConstraintName("FK_Bets_Users")
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(b => b.Game)
                .WithMany(g => g.Bets)
                .HasForeignKey(b => b.GameId)
                .HasConstraintName("FK_Bets_Games")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
