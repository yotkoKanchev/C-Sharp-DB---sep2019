namespace PetClinic.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Animal
    {
        public int Id { get; set; }

        [MinLength(3), MaxLength(20), Required]
        public string Name { get; set; }

        [MinLength(3), MaxLength(20), Required]
        public string Type { get; set; }

        [Range(1, int.MaxValue)]
        public int Age { get; set; }

        [ForeignKey(nameof(Passport))] //one to one realation FK
        public string PassportSerialNumber { get; set; }

        public Passport Passport { get; set; }

        public ICollection<Procedure> Procedures { get; set; } = new HashSet<Procedure>();
    }
}
