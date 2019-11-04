namespace P01_HospitalDatabase.Data 
{
    using Microsoft.EntityFrameworkCore;
    using P01_HospitalDatabase.Data.Models;

    public class HospitalContext : DbContext
    {
        public DbSet<Patient> Patients { get; set; }

        public DbSet<Visitation> Visitations { get; set; }

        public DbSet<Medicament> Medicaments { get; set; }

        public DbSet<Diagnose> Diagnoses { get; set; }

        public DbSet<PatientMedicament> PatientsMedicaments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DataSettings.DefaultConnection);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(p => p.PatientId);

                //entity.ToTable("Patients");

                entity.Property(p => p.FirstName)
                      .HasMaxLength(50)
                      .IsRequired(true)
                      .IsUnicode(true);

                entity.Property(p => p.LastName)
                     .HasMaxLength(50)
                     .IsRequired(true)
                     .IsUnicode(true);

                entity.Property(p => p.Address)
                     .HasMaxLength(250)
                     .IsRequired(false)
                     .IsUnicode(true);

                entity.Property(p => p.Email)
                     .HasMaxLength(80)
                     .IsRequired(false)
                     .IsUnicode(false);

                entity.Property(p => p.HasInsurance)
                     .IsRequired(true);
            });

            modelBuilder.Entity<Visitation>(entity =>
            {
                entity.HasKey(v => v.VisitationId);

                //entity.ToTable("Visitations");

                entity.Property(v => v.Date)
                      .IsRequired(true);

                entity.Property(v => v.Comments)
                      .HasMaxLength(250)
                      .IsRequired(false)
                      .IsUnicode(true);

                entity.HasOne(v => v.Patient)
                      .WithMany(p => p.Visitations)
                      .HasForeignKey(v => v.PatientId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_Visitations_Patients");

                entity.HasOne(v => v.Doctor)
                      .WithMany(d => d.Visitations)
                      .HasForeignKey(v => v.DoctorId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_Visitations_Doctors");
            });

            modelBuilder.Entity<Diagnose>(entity =>
            {
                entity.HasKey(d => d.DiagnoseId);

                //entity.ToTable("Diagnoses");

                entity.Property(d => d.Name)
                      .HasMaxLength(50)
                      .IsRequired(true)
                      .IsUnicode(true);

                entity.Property(d => d.Comments)
                      .HasMaxLength(250)
                      .IsRequired(false)
                      .IsUnicode(true);

                entity.HasOne(d => d.Patient)
                      .WithMany(p => p.Diagnoses)
                      .HasForeignKey(d => d.PatientId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_Diagnosess_Patients");
            });

            modelBuilder.Entity<Medicament>(entity =>
            {
                entity.HasKey(m => m.MedicamentId);

                //entity.ToTable("Medicaments");

                entity.Property(m => m.Name)
                      .HasMaxLength(50)
                      .IsRequired(true)
                      .IsUnicode(true);
            });

            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasKey(d => d.DoctorId);

                //entity.ToTable("Doctors");

                entity.Property(d => d.Name)
                      .HasMaxLength(100)
                      .IsRequired(true)
                      .IsUnicode(true);

                entity.Property(d => d.Specialty)
                      .HasMaxLength(100)
                      .IsRequired(true)
                      .IsUnicode(true);
            });

            modelBuilder.Entity<PatientMedicament>(entity =>
            {
                entity.HasKey(pm => new
                {
                    pm.PatientId,
                    pm.MedicamentId
                });

                //entity.ToTable("PatientsMedicaments");

                entity.HasOne(pm => pm.Patient)
                      .WithMany(p => p.Prescriptions)
                      .HasForeignKey(pm => pm.PatientId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_PatientsMedicaments_Patients");

                entity.HasOne(pm => pm.Medicament)
                      .WithMany(m => m.Prescriptions)
                      .HasForeignKey(pm => pm.MedicamentId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_PatientsMedicaments_Medicaments");
            });
        }
    }
}
