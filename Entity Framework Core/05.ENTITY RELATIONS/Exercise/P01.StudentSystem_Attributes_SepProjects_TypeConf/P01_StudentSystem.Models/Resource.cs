namespace P01_StudentSystem.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using Enumerations;

    using static DataValidations.Resource;

    public class Resource
    {
        [Key]
        public int ResourceId { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; }

        [Required]
        [MaxLength(UrlMaxLength)]
        public string Url { get; set; }

        [Required]
        public ResourceType ResourceType { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
