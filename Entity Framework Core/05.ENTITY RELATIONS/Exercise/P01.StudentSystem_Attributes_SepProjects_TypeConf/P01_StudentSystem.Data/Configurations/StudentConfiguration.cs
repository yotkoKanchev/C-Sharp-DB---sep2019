namespace P01_StudentSystem.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Data.Models;
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder
                .Property(e => e.PhoneNumber)
                .IsFixedLength(true)
                .IsUnicode(false);
        }
    }
}
