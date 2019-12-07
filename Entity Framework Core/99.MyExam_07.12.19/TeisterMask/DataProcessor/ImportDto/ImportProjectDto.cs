namespace TeisterMask.DataProcessor.ImportDto
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Project")]
    public class ImportProjectDto
    {
        [MinLength(2), MaxLength(40), Required]
        public string Name { get; set; }

        [MinLength(10), MaxLength(10), Required]
        public string OpenDate { get; set; }

        public string DueDate { get; set; }

        public ImportTaskDto[] Tasks { get; set; }
    }
}
