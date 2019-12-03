namespace SoftJail.DataProcessor.ExportDto
{
    using System.Xml.Serialization;

    [XmlType("Prisoner")]
    public class ExportPrisonerDto
    {
        [XmlElement]
        public int Id { get; set; }

        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string IncarcerationDate { get; set; }

        [XmlArray]
        public ExportMessageDto[] EncryptedMessages { get; set; }
    }
}
