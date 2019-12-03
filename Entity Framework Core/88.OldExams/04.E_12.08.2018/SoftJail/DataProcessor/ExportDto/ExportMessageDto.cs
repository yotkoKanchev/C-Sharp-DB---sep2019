namespace SoftJail.DataProcessor.ExportDto
{
    using System.Xml.Serialization;

    [XmlType("Message")]
    public class ExportMessageDto
    {
        [XmlElement]
        public string Description { get; set; }
    }
}