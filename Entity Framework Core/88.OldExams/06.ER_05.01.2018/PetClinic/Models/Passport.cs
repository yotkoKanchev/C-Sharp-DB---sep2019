namespace PetClinic.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Passport
    {
        [Key]
        [RegularExpression(@"^[A-Za-z]{7}[0-9]{3}$"), Required]
        public string SerialNumber { get; set; }

        [Required]
        public Animal Animal { get; set; }

        [RegularExpression(@"^((\+359)|(0))[0-9]{9}$"), Required]
        public string OwnerPhoneNumber { get; set; }

        [MinLength(3), MaxLength(30), Required]
        public string OwnerName { get; set; }

        public DateTime RegistrationDate { get; set; }
    }
}
