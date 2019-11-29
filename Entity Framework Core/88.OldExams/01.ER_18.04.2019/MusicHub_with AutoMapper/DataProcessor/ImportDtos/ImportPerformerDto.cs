namespace MusicHub.DataProcessor.ImportDtos
{
    using System.Xml.Serialization;

    [XmlType("Performer")]
    public class ImportPerformerDto
    {
        [XmlElement]
        public string FirstName { get; set; }

        [XmlElement]
        public string LastName { get; set; }

        [XmlElement]
        public int Age { get; set; }

        [XmlElement]
        public decimal NetWorth { get; set; }

        [XmlArray("PerformersSongs")]
        public ImportSongIdDto[] Songs { get; set; }
    }
}
