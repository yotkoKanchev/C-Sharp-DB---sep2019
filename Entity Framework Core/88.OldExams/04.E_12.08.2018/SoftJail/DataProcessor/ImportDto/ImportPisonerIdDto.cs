namespace SoftJail.DataProcessor.ImportDto
{
    using System.Xml.Serialization;

    [XmlType("Prisoner")]
    public class ImportPrisonerIdDto
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
    }
}
