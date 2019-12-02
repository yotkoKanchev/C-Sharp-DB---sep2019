namespace SoftJail.DataProcessor.ImportDto
{
    using System.ComponentModel.DataAnnotations;

    public class ImportDepartmentDto
    {
        [MinLength(3), MaxLength(25), Required]
        public string Name { get; set; }

        public ImportCellDto[] Cells { get; set; }
    }
}
