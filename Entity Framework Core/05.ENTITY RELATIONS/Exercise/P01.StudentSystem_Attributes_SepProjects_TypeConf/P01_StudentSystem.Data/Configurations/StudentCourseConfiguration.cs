namespace P01_StudentSystem.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Data.Models;
    public class StudentCourseConfiguration : IEntityTypeConfiguration<StudentCourse>
    {
        public void Configure(EntityTypeBuilder<StudentCourse> builder)
        {
            builder
                .HasKey(e => new { e.CourseId, e.StudentId });

            builder
                .HasOne(s => s.Course)
                .WithMany(c => c.StudentsEnrolled)
                .HasForeignKey(s => s.CourseId);

            builder
                .HasOne(c => c.Student)
                .WithMany(s => s.CourseEnrollments)
                .HasForeignKey(c => c.StudentId);
        }
    }
}
