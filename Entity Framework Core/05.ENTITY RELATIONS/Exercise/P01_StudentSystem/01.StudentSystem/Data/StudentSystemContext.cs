namespace P01_StudentSystem.Data
{
    using Microsoft.EntityFrameworkCore;
    using Models;

    using static DataValidations;
    public class StudentSystemContext : DbContext
    {
        // to be able to run StudentSystemContext in StartUp
        public StudentSystemContext()
        {

        }
        // Judge needs it !!!
        public StudentSystemContext(DbContextOptions options)
        : base(options)
        {

        }
        public DbSet<Resource> Resources { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Homework> HomeworkSubmissions { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<StudentCourse> StudentCourses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DataSettings.DefaultConnection);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Student>(entity =>
            {
                entity
                    .HasKey(e => e.StudentId);

                entity
                    .Property(e => e.Name)
                    .HasMaxLength(StudentMaxNameLength)
                    .IsRequired(true)
                    .IsUnicode(true);

                entity
                    .Property(e => e.PhoneNumber)
                    .HasMaxLength(PhoneNumberFixedLength)
                    .IsFixedLength(true)
                    .IsRequired(false)
                    .IsUnicode(false);

                entity
                    .Property(e => e.RegisteredOn)
                    .IsRequired(true);

                entity
                    .Property(e => e.Birthday)
                    .IsRequired(false);
            });

            builder.Entity<Course>(entity =>
            {
                entity
                    .HasKey(e => e.CourseId);

                entity
                    .Property(e => e.Name)
                    .HasMaxLength(CourseMaxNameLength)
                    .IsRequired(true)
                    .IsUnicode(true);

                entity
                    .Property(e => e.Description)
                    .HasMaxLength(DescriptionMaxLength)
                    .IsRequired(false)
                    .IsUnicode(true);

                entity
                    .Property(e => e.StartDate)
                    .IsRequired(true);

                entity
                    .Property(e => e.EndDate)
                    .IsRequired(true);

                entity
                    .Property(e => e.Price)
                    .IsRequired(true);
            });

            builder.Entity<Resource>(entity =>
            {
                entity
                    .HasKey(e => e.ResourceId);

                entity
                    .Property(e => e.Name)
                    .HasMaxLength(ResourceNameMaxLength)
                    .IsRequired(true)
                    .IsUnicode(true);

                entity
                    .Property(e => e.Url)
                    .HasMaxLength(UrlMaxLength)
                    .IsRequired(true)
                    .IsUnicode(false);

                entity
                    .Property(e => e.ResourceType)
                    .IsRequired(true);

                entity
                    .HasOne(c => c.Course)
                    .WithMany(r => r.Resources)
                    .HasForeignKey(c => c.CourseId)
                    .HasConstraintName("FK_Courses_Resources")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Homework>(entity =>
            {
                entity
                    .ToTable("HomeworkSubmissions");

                entity
                    .HasKey(e => e.HomeworkId);

                entity
                    .Property(e => e.Content)
                    .HasMaxLength(ContentMaxLength)
                    .IsRequired(true)
                    .IsUnicode(false);

                entity
                    .Property(e => e.ContentType)
                    .IsRequired(true);

                entity
                    .Property(e => e.SubmissionTime)
                    .IsRequired(true);

                entity
                    .HasOne(e => e.Student)
                    .WithMany(s => s.HomeworkSubmissions)
                    .HasForeignKey(e => e.StudentId)
                    .HasConstraintName("FK_Students_Homeworks")
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(e => e.Course)
                    .WithMany(c => c.HomeworkSubmissions)
                    .HasForeignKey(e => e.CourseId)
                    .HasConstraintName("FK_Courses_Homeworks")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<StudentCourse>(entity =>
            {
                entity
                    .HasKey(e => new { e.StudentId, e.CourseId });

                entity
                    .HasOne(s => s.Student)
                    .WithMany(sc => sc.CourseEnrollments)
                    .HasForeignKey(s => s.StudentId)
                    .HasConstraintName("FK_StudentCources_Students")
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(c => c.Course)
                    .WithMany(sc => sc.StudentsEnrolled)
                    .HasForeignKey(c => c.CourseId)
                    .HasConstraintName("FK_StudentCources_Cources")
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
