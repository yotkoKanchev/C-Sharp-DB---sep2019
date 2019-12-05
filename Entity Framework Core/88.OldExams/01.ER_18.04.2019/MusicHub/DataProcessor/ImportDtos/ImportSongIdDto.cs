namespace MusicHub.DataProcessor.ImportDtos
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Song")]
    public class ImportSongIdDto
    {
        [XmlAttribute("id")]
        [Range(0, int.MaxValue)]
        public int Id { get; set; }
    }
}
