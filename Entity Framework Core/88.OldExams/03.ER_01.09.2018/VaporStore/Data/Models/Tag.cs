namespace VaporStore.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Tag
    {
        [Key, Required]
        public int Id { get; set; }

        [MaxLength(100), Required]
        public string Name { get; set; }

        public ICollection<GameTag> GameTags { get; set; } = new HashSet<GameTag>();
    }
}
