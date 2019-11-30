namespace VaporStore.DataProcessor.ImportDtos
{
    using System.ComponentModel.DataAnnotations;

    public class ImportCardDto
    {
        [RegularExpression(@"^\d{4} \d{4} \d{4} \d{4}$"), Required]
        public string Number { get; set; }

        [RegularExpression(@"^\d{3}$"), Required]
        public string Cvc { get; set; }

        [Required]
        public string Type { get; set; }
    }
}
