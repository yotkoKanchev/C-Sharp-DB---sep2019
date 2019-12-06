namespace Cinema.DataProcessor.ImportDto
{
    using System.ComponentModel.DataAnnotations;

    public class ImportMovieDto
    {
        [MinLength(3), MaxLength(20), Required]
        public string Title { get; set; }

        public string Genre { get; set; }

        public string Duration { get; set; }

        [Range(1, 10)]
        public double Rating { get; set; }

        [MinLength(3), MaxLength(20), Required]
        public string Director { get; set; }
    }
}
