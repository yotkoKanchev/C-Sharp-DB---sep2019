namespace P03_FootballBetting.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static DataValidations.Color;
    public class Color
    {
        [Key]
        public int ColorId { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; }

        public ICollection<Team> PrimaryKitTeams { get; set; } = new HashSet<Team>();

        public ICollection<Team> SecondaryKitTeams { get; set; } = new HashSet<Team>();
    }
}
