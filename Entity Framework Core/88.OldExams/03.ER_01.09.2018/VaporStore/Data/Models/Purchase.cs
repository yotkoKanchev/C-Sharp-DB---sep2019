namespace VaporStore.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Enumerations;

    public class Purchase
    {
        [Key, Required] 
        public int Id { get; set; }

        [Required]
        public PurchaseType Type { get; set; }

        [RegularExpression(@"^[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$"), Required]
        public string ProductKey { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [ForeignKey(nameof(Card)), Required]
        public int CardId { get; set; }

        [Required]
        public Card Card { get; set; }

        [ForeignKey(nameof(Game)), Required]
        public int GameId { get; set; }

        [Required]
        public Game Game { get; set; }
    }
}
