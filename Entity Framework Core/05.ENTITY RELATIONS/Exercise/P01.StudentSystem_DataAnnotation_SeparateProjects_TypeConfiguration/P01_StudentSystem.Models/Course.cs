namespace P01_StudentSystem.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static DataValidations.Course;
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; }

        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public decimal Price { get; set; }

        public ICollection<StudentCourse> StudentsEnrolled { get; set; } = new HashSet<StudentCourse>();

        public ICollection<Resource> Resources { get; set; } = new HashSet<Resource>();

        public ICollection<Homework> HomeworkSubmissions { get; set; } = new HashSet<Homework>();
    }
}
