namespace FastFood.DataProcessor.Dto.Import
{
    using System.ComponentModel.DataAnnotations;

    class ImportEmployeeDto
    {
        [MinLength(3), MaxLength(30), Required]
        public string Name { get; set; }

        [Range(15, 80)]
        public int Age { get; set; }

        [MinLength(3), MaxLength(30), Required]
        public string Position { get; set; }
    }
}
