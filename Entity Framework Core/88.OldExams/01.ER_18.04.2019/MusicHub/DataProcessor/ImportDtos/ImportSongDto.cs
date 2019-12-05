namespace MusicHub.DataProcessor.ImportDtos
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Song")]
    public class ImportSongDto
    {
        [MinLength(3), MaxLength(20), Required]
        public string Name { get; set; }

        [MinLength(8)]
        public string Duration { get; set; }

        [MinLength(10)]
        public string CreatedOn { get; set; }

        [MinLength(3), Required]
        public string Genre { get; set; }

        [Range(0, int.MaxValue)]
        public int? AlbumId { get; set; }

        [Range(0, int.MaxValue)]
        public int WriterId { get; set; }
    }
}
