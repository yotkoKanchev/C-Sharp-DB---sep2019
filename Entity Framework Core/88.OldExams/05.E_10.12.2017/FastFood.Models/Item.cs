namespace FastFood.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Item
    {
        public int Id { get; set; }

        [MinLength(3), MaxLength(30), Required] // TODO needs to be unique !!!
        public string Name { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();

    }
}
