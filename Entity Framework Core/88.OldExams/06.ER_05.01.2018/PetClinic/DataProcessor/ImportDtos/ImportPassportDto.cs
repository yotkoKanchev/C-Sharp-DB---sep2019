namespace PetClinic.DataProcessor.ImportDtos
{
    using System.ComponentModel.DataAnnotations;

    public class ImportPassportDto
    {
        [RegularExpression(@"^[A-Za-z]{7}[0-9]{3}$"), Required]
        public string SerialNumber { get; set; }

        [MinLength(3), MaxLength(30), Required]
        public string OwnerName { get; set; }

        [RegularExpression(@"^((\+359)|(0))[0-9]{9}$"), Required]
        public string OwnerPhoneNumber { get; set; }

        public string RegistrationDate { get; set; }
    }
}
