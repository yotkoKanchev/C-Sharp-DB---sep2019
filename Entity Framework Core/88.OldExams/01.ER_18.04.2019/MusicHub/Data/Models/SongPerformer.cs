namespace MusicHub.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class SongPerformer
    {
        [Required]
        [ForeignKey(nameof(Song))]
        public int SongId { get; set; }

        public Song Song { get; set; }

        [Required]
        [ForeignKey(nameof(Performer))]
        public int PerformerId { get; set; }

        public Performer Performer { get; set; }
    }
}
