namespace PetClinic.DataProcessor.ImportDtos
{
    using System.ComponentModel.DataAnnotations;

    class ImportAnimalDto
    {
        [MinLength(3), MaxLength(20), Required]
        public string Name { get; set; }

        [MinLength(3), MaxLength(20), Required]
        public string Type { get; set; }

        [Range(1, int.MaxValue)]
        public int Age { get; set; }

        public ImportPassportDto Passport { get; set; }
    }
}
