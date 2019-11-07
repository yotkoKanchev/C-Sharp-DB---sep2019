namespace P01_StudentSystem.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Data.Models;
    public class HomeworkConfiguration : IEntityTypeConfiguration<Homework>
    {
        public void Configure(EntityTypeBuilder<Homework> builder)
        {
            builder
                .Property(e => e.Content)
                .IsUnicode(false);

            builder
                .HasOne(h => h.Student)
                .WithMany(s => s.HomeworkSubmissions)
                .HasForeignKey(h => h.StudentId);

            builder
               .HasOne(h => h.Course)
               .WithMany(c => c.HomeworkSubmissions)
               .HasForeignKey(h => h.CourseId);
        }
    }
}
