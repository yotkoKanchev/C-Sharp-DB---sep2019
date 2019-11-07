namespace P03_FootballBetting.Data
{
    using Microsoft.EntityFrameworkCore;
    using P03_FootballBetting.Data.Models;

    using static DataSettings;
    using static DataValidations;
    public class FootballBettingContext : DbContext
    {
        // to be able runing FootballBettingContext in StartUp
        public FootballBettingContext()
        {

        }
        // Judge needs it !!!
        public FootballBettingContext(DbContextOptions options)
        : base(options)
        {

        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Bet> Bets { get; set; }
        public DbSet<PlayerStatistic> PlayerStatistics { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DefaultConnection);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Country>(entity =>
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
            });

            builder.Entity<Color>(entity =>
            {
                entity
                    .HasKey(e => e.ColorId);

                entity
                    .Property(e => e.Name)
                    .HasMaxLength(ColorMaxNameLength)
                    .IsRequired(true)
                    .IsUnicode(true);
            });

            builder.Entity<Position>(entity =>
            {
                entity
                    .HasKey(e => e.PositionId);

                entity
                    .Property(e => e.Name)
                    .HasMaxLength(PositionMaxNameLength)
                    .IsRequired(true)
                    .IsUnicode(true);
            });

            builder.Entity<User>(entity =>
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
            });

            builder.Entity<Town>(entity =>
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
            });

            builder.Entity<Team>(entity =>
            {
                entity
                    .HasKey(e => e.TeamId);

                entity
                    .Property(e => e.Name)
                    .HasMaxLength(TeamMaxNameLength)
                    .IsRequired(true)
                    .IsUnicode(true);

                entity
                    .Property(e => e.LogoUrl)
                    .HasMaxLength(LogoUrlMaxLength)
                    .IsRequired(false)
                    .IsUnicode(true);

                entity
                    .Property(e => e.Initials)
                    .HasMaxLength(InitialsUrlMaxLength)
                    .IsRequired(false)
                    .IsUnicode(true);

                entity
                    .Property(e => e.Budget)
                    .IsRequired(true);

                entity
                    .HasOne(t => t.Town)
                    .WithMany(to => to.Teams)
                    .HasForeignKey(t => t.TownId)
                    .HasConstraintName("FK_Teams_Towns")
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(t => t.PrimaryKitColor)
                    .WithMany(pc => pc.PrimaryKitTeams)
                    .HasForeignKey(t => t.PrimaryKitColorId)
                    .HasConstraintName("FK_Teams_Colors_PrimaryTeams")
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(t => t.SecondaryKitColor)
                    .WithMany(sc => sc.SecondaryKitTeams)
                    .HasForeignKey(t => t.SecondaryKitColorId)
                    .HasConstraintName("FK_Teams_Colors_SecondaryTeams");
            });

            builder.Entity<Player>(entity => 
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
            });

            builder.Entity<Game>(entity =>
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
            });

            builder.Entity<Bet>(entity =>
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
            });

            builder.Entity<PlayerStatistic>(entity =>
            {
                entity
                    .HasKey(e => new { e.PlayerId, e.GameId });

                entity
                    .ToTable("PlayerStatistics");

                entity
                    .Property(e => e.ScoredGoals)
                    .IsRequired(true);

                entity
                    .Property(e => e.Assists)
                    .IsRequired(true);

                entity
                    .Property(e => e.MinutesPlayed)
                    .IsRequired(true);

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
            });          
        }
    }
}
