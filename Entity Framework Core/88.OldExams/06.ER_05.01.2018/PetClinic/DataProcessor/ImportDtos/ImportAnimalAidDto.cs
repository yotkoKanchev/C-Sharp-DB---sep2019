namespace PetClinic.DataProcessor.ImportDtos
{
    using System.ComponentModel.DataAnnotations;

    public class ImportAnimalAidDto
    {
        [MinLength(3), MaxLength(30), Required]
        public string Name { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Price { get; set; }
    }
}
