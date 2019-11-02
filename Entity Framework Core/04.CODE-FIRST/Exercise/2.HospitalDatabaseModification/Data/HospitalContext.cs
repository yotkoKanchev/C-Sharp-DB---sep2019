namespace P01_HospitalDatabase.Data
{
    using Microsoft.EntityFrameworkCore;
    using P01_HospitalDatabase.Data.Models;

    public class HospitalContext : DbContext
    {
        public HospitalContext()
        {

        }

        public HospitalContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<Diagnose> Diagnoses { get; set; }

        public DbSet<Visitation> Visitations { get; set; }

        public DbSet<Medicament> Medicaments { get; set; }

        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<PatientMedicament> PatientsMedicaments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DataSettings.DafaultConnection);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Patient>(entity =>
                {
                    entity
                        .HasKey(e => e.PatientId);

                    //entity.ToTable("Patients");

                    entity
                        .Property(e => e.FirstName)
                        .HasMaxLength(50)
                        .IsRequired(true)
                        .IsUnicode(true);

                    entity
                        .Property(e => e.LastName)
                        .HasMaxLength(50)
                        .IsRequired(true)
                        .IsUnicode(true);

                    entity
                        .Property(e => e.Address)
                        .HasMaxLength(250)
                        .IsRequired(false)
                        .IsUnicode(true);

                    entity
                        .Property(e => e.Email)
                        .HasMaxLength(80)
                        .IsRequired(false)
                        .IsUnicode(false);

                    entity
                        .Property(e => e.HasInsurance)
                        .IsRequired(true);
                });

            modelBuilder
                .Entity<Visitation>(entity =>
                {
                    entity
                        .HasKey(e => e.VisitationId);

                    //entity.ToTable("Visitations");

                    entity
                        .Property(e => e.Date)
                        .IsRequired(true);

                    entity
                        .Property(e => e.Comments)
                        .HasMaxLength(250)
                        .IsRequired(false)
                        .IsUnicode(false);

                    entity
                        .HasOne(p => p.Patient)
                        .WithMany(v => v.Visitations)
                        .HasForeignKey(p => p.PatientId);

                    entity
                        .HasOne(d => d.Doctor)
                        .WithMany(v => v.Visitations)
                        .HasForeignKey(d => d.DoctorId);
                });

            modelBuilder
                .Entity<Diagnose>(entity =>
                {
                    entity
                        .HasKey(e => e.DiagnoseId);

                    //entity.ToTable("Diagnoses");

                    entity
                        .Property(e => e.Name)
                        .HasMaxLength(50)
                        .IsRequired(true)
                        .IsUnicode(true);

                    entity
                        .Property(e => e.Comments)
                        .HasMaxLength(250)
                        .IsRequired(true)
                        .IsUnicode(true);

                    entity
                        .HasOne(p => p.Patient)
                        .WithMany(d => d.Diagnoses)
                        .HasForeignKey(p => p.PatientId);
                });

            modelBuilder
                .Entity<Medicament>(entity =>
                {
                    entity
                        .HasKey(e => e.MedicamentId);

                    //entity.ToTable("Medicaments");

                    entity
                        .Property(e => e.Name)
                        .HasMaxLength(50)
                        .IsRequired(true)
                        .IsUnicode(true);
                });

            modelBuilder
                .Entity<PatientMedicament>(entity =>
                {
                    entity
                        .HasKey(e => new
                        {
                            e.PatientId,
                            e.MedicamentId
                        });

                    entity
                        .Property(e => e.PatientId);
                    //.HasColumnName("PatientId");

                    entity
                        .Property(e => e.MedicamentId);
                    //.HasColumnName("MedicamentId");

                    entity
                        .HasOne(m => m.Patient)
                        .WithMany(p => p.Prescriptions)
                        .HasForeignKey(m => m.PatientId);

                    entity
                        .HasOne(p => p.Medicament)
                        .WithMany(m => m.Prescriptions)
                        .HasForeignKey(p => p.MedicamentId);
                });

            modelBuilder
                    .Entity<Doctor>(entity =>
                    {
                        entity.HasKey(e => e.DoctorId);

                        entity
                            .Property(e => e.Name)
                            .HasMaxLength(100)
                            .IsRequired(true)
                            .IsUnicode(true);

                        entity
                            .Property(e => e.Specialty)
                            .HasMaxLength(100)
                            .IsRequired(true)
                            .IsUnicode(true);
                    });
        }
    }
}
