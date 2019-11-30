namespace VaporStore.DataProcessor.ImportDtos
{
    using System.ComponentModel.DataAnnotations;

    public class ImportUserDto
    {
        [RegularExpression(@"^[A-Z]{1}[a-z]+ [A-Z]{1}[a-z]+$"), Required]
        public string FullName { get; set; }

        [MinLength(3), MaxLength(20), Required]
        public string Username { get; set; }


        [MaxLength(100), Required]
        public string Email { get; set; }

        [Range(3, 103), Required]
        public int Age { get; set; }

        public ImportCardDto[] Cards { get; set; }
    }
}
