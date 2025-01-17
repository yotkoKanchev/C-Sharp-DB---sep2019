﻿namespace VaporStore.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Developer
    {
        [Key, Required]
        public int Id { get; set; }

        [MaxLength(100), Required]
        public string Name { get; set; }

        public ICollection<Game> Games { get; set; } = new HashSet<Game>();
    }
}
