namespace TeisterMask.DataProcessor.ImportDto
{
    using System.ComponentModel.DataAnnotations;

    public class ImportEmployeeDto
    {
        [RegularExpression(@"^[A-Za-z0-9]{3,40}$"), Required]
        public string Username { get; set; }

        [EmailAddress, Required]
        public string Email { get; set; }

        [RegularExpression(@"^[0-9]{3}-[0-9]{3}-[0-9]{4}$"), Required]
        public string Phone { get; set; }

        public int[] Tasks { get; set; }
    }
}
