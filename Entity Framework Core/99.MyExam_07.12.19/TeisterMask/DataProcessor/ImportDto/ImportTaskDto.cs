namespace TeisterMask.DataProcessor.ImportDto
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Task")]
    public class ImportTaskDto
    {
        [XmlElement]
        [MinLength(2), MaxLength(40), Required]
        public string Name { get; set; }

        [XmlElement]
        [MinLength(10), MaxLength(10), Required]
        public string OpenDate { get; set; }

        [XmlElement]
        [MinLength(10), MaxLength(10), Required]
        public string DueDate { get; set; }

        [XmlElement]
        [Required]
        public int ExecutionType { get; set; }

        [XmlElement]
        [Required]
        public int LabelType { get; set; }
    }
}
