namespace Cinema.DataProcessor.ImportDto
{
    using System;
    using Data.Models.Enums;
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    public class ImportMovieDto
    {
        //[JsonProperty, MinLength(3), MaxLength(20)]
        public string Title { get; set; }

        //[JsonProperty]
        public string Genre { get; set; }

        //[JsonProperty]
        public string Duration { get; set; }

        //[JsonProperty, MinLength(1), MaxLength(10)]
        public double Rating { get; set; }

        //[JsonProperty, MinLength(3), MaxLength(20)]
        public string Director { get; set; }
    }
}
