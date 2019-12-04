namespace PetClinic.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AnimalAid
    {
        public int Id { get; set; }

        //needs to be unique
        [MinLength(3), MaxLength(30), Required]
        public string Name { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        public ICollection<ProcedureAnimalAid> AnimalAidProcedures { get; set; } = new HashSet<ProcedureAnimalAid>();
    }
}
