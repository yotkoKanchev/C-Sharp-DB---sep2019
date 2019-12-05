namespace MusicHub.DataProcessor.ExportDtos
{
    using System.Xml.Serialization;

    [XmlType("Song")]
    public class ExportSongAboveDurationDto
    {
        [XmlElement]
        public string SongName { get; set; }

        [XmlElement]
        public string Writer { get; set; }

        [XmlElement]
        public string Performer { get; set; }

        [XmlElement]
        public string AlbumProducer { get; set; }

        [XmlElement]
        public string Duration { get; set; }
    }
}
