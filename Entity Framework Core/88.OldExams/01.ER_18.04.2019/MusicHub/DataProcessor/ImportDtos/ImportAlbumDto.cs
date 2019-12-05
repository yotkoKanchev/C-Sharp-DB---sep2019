namespace MusicHub.DataProcessor.ImportDtos
{
    using System.ComponentModel.DataAnnotations;

    public class ImportAlbumDto
    {
        [MinLength(3), MaxLength(40), Required]
        public string Name { get; set; }
      
        [MinLength(6)]
        public string ReleaseDate { get; set; }
    }
}