namespace VaporStore.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Enumerations;

    public class Card
    {
        [Key, Required]
        public int Id { get; set; }

        [RegularExpression(@"^\d{4} \d{4} \d{4} \d{4}$"), Required]
        public string Number { get; set; }

        [RegularExpression(@"^\d{3}$"), Required]
        public string Cvc { get; set; }

        [Required]
        public CardType Type { get; set; }

        [ForeignKey(nameof(User)), Required]
        public int UserId { get; set; }

        [Required]
        public User User { get; set; }

        public ICollection<Purchase> Purchases { get; set; } = new HashSet<Purchase>();
    }
}
