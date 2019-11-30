namespace VaporStore.DataProcessor.ImportDtos
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Purchase")]
    public class ImportPurchaseDto
    {
        [XmlAttribute("title")]
        public string Title { get; set; }

        [XmlElement]
        public string Type { get; set; }

        [XmlElement]
        [RegularExpression(@"^[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$"), Required]
        public string Key { get; set; }

        [XmlElement]
        [RegularExpression(@"^\d{4} \d{4} \d{4} \d{4}$"), Required]
        public string Card { get; set; }

        [XmlElement]
        public string Date { get; set; }
    }
}
