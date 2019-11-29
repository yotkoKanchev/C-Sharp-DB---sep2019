namespace Cinema.DataProcessor.ImportDto
{
    using System.Xml.Serialization;

    [XmlType("Projection")]
    public class ImportProjectionDto
    {
        [XmlElement]
        public int MovieId { get; set; }

        [XmlElement]
        public int HallId { get; set; }

        [XmlElement]
        public string DateTime { get; set; }
    }
}
