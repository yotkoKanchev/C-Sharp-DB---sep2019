namespace P03_FootballBetting.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static DataValidations.Team;
    public class Team
    {
        [Key]
        public int TeamId { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; }

        [MaxLength(LogoUrlMaxLength)]
        public string LogoUrl { get; set; }

        [MaxLength(InitialsUrlMaxLength)]
        public string Initials { get; set; }

        [Required]
        public decimal Budget { get; set; }

        public int PrimaryKitColorId { get; set; } 
        public Color PrimaryKitColor { get; set; }

        public int SecondaryKitColorId { get; set; }
        public Color SecondaryKitColor { get; set; }

        public int TownId { get; set; }
        public Town Town { get; set; }

        public ICollection<Game> HomeGames { get; set; } = new HashSet<Game>();
        public ICollection<Game> AwayGames { get; set; } = new HashSet<Game>();

        public ICollection<Player> Players { get; set; } = new HashSet<Player>();
    }
}
