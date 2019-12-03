namespace FastFood.DataProcessor.Dto.Import
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Order")]
    public class ImportOrderDto
    {
        [XmlElement]
        [Required]
        public string Customer { get; set; }

        [XmlElement]
        [Required]
        public string Employee { get; set; }

        [XmlElement]
        public string DateTime { get; set; }

        [XmlElement]
        [Required]
        public string Type { get; set; }

        [XmlArray]
        public ItemDto[] Items { get; set; }
    }
}
