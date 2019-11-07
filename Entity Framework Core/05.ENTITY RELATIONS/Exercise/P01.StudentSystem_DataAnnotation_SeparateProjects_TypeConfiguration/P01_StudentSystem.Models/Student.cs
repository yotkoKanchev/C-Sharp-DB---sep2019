namespace P01_StudentSystem.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static DataValidations.Student;
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; }

        [MaxLength(PhoneFixedLength)]
        public string PhoneNumber { get; set; }

        [Required]
        public DateTime RegisteredOn { get; set; }

        public DateTime? Birthday { get; set; }

        public ICollection<StudentCourse> CourseEnrollments { get; set; } = new HashSet<StudentCourse>();

        public ICollection<Homework> HomeworkSubmissions { get; set; }  = new HashSet<Homework>();
    }
}
