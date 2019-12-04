namespace PetClinic.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Vet
    {
        public int Id { get; set; }

        [MinLength(3), MaxLength(40), Required]
        public string Name { get; set; }

        [MinLength(3), MaxLength(50), Required]
        public string Profession { get; set; }

        [Range(22, 65)]
        public int Age { get; set; }

        //needs to be unique - EFC fluent API
        [RegularExpression(@"^((\+359)|(0))[0-9]{9}$"), Required]
        public string PhoneNumber { get; set; }

        public ICollection<Procedure> Procedures { get; set; } = new HashSet<Procedure>();
    }
}
