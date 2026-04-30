using IVF_Managment_Api.Models;
using IVF_Managment_Api.Models.BaseModel;
using IVF_Managment_Api.Models.HelperModels;
using Microsoft.EntityFrameworkCore;

namespace IVF_Managment_Api.Data;

public class IvfDbContext : DbContext
{
    public IvfDbContext(DbContextOptions<IvfDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<LabTechnician> LabTechnicians => Set<LabTechnician>();
    public DbSet<Administrator> Administrators => Set<Administrator>();

    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<AvailabilitySchedule> AvailabilitySchedules => Set<AvailabilitySchedule>();

    public DbSet<Bill> Bills => Set<Bill>();
    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<IvfCycle> IvfCycles => Set<IvfCycle>();
    public DbSet<Embryo> Embryos => Set<Embryo>();
    public DbSet<EmbryoObservation> EmbryoObservations => Set<EmbryoObservation>();

    public DbSet<LabTestOrder> LabTestOrders => Set<LabTestOrder>();
    public DbSet<LabTestReport> LabTestReports => Set<LabTestReport>();
    public DbSet<MedicalRecordEntry> MedicalRecordEntries => Set<MedicalRecordEntry>();
    public DbSet<ChainOfCustodyLog> ChainOfCustodyLogs => Set<ChainOfCustodyLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Table-Per-Type for the User hierarchy: each derived role gets its own table.
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Patient>().ToTable("Patients");
        modelBuilder.Entity<Doctor>().ToTable("Doctors");
        modelBuilder.Entity<LabTechnician>().ToTable("LabTechnicians");
        modelBuilder.Entity<Administrator>().ToTable("Administrators");

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<Patient>()
            .HasIndex(p => p.NationalIdNumber)
            .IsUnique();
        modelBuilder.Entity<Patient>()
            .HasIndex(p => p.PatientSystemId)
            .IsUnique();

        modelBuilder.Entity<Doctor>()
            .HasIndex(d => d.LicenseNumber)
            .IsUnique();

        modelBuilder.Entity<Bill>()
            .HasIndex(b => b.InvoiceNumber)
            .IsUnique();

        // Patient ↔ Doctor (one assigned doctor per patient, optional)
        modelBuilder.Entity<Patient>()
            .HasOne(p => p.AssignedDoctor)
            .WithMany(d => d.AssignedPatients)
            .HasForeignKey(p => p.AssignedDoctorId)
            .OnDelete(DeleteBehavior.SetNull);

        // Appointment relationships
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Bill ↔ Patient
        modelBuilder.Entity<Bill>()
            .HasOne(b => b.Patient)
            .WithMany(p => p.Bills)
            .HasForeignKey(b => b.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Payment ↔ Bill
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Bill)
            .WithMany(b => b.Payments)
            .HasForeignKey(p => p.BillId)
            .OnDelete(DeleteBehavior.Cascade);

        // IvfCycle relationships
        modelBuilder.Entity<IvfCycle>()
            .HasOne(c => c.Patient)
            .WithMany()
            .HasForeignKey(c => c.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<IvfCycle>()
            .HasOne(c => c.AssignedDoctor)
            .WithMany()
            .HasForeignKey(c => c.AssignedDoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Embryo ↔ IvfCycle ↔ EmbryoObservation
        modelBuilder.Entity<Embryo>()
            .HasOne(e => e.IvfCycle)
            .WithMany(c => c.Embryos)
            .HasForeignKey(e => e.IvfCycleId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<EmbryoObservation>()
            .HasOne(o => o.Technician)
            .WithMany(t => t.EmbryoObservations)
            .HasForeignKey(o => o.TechnicianId)
            .OnDelete(DeleteBehavior.Restrict);

        // LabTestOrder relationships (Patient + Requesting Doctor + optional Fulfilling Tech)
        modelBuilder.Entity<LabTestOrder>()
            .HasOne(o => o.Patient)
            .WithMany()
            .HasForeignKey(o => o.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LabTestOrder>()
            .HasOne(o => o.RequestingDoctor)
            .WithMany()
            .HasForeignKey(o => o.RequestingDoctorId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LabTestOrder>()
            .HasOne(o => o.FulfillingTechnician)
            .WithMany()
            .HasForeignKey(o => o.FulfillingTechnicianId)
            .OnDelete(DeleteBehavior.SetNull);

        // LabTestReport ↔ Technician (UploadedTests)
        modelBuilder.Entity<LabTestReport>()
            .HasOne(r => r.Technician)
            .WithMany(t => t.UploadedTests)
            .HasForeignKey(r => r.TechnicianId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LabTestReport>()
            .HasOne(r => r.Patient)
            .WithMany()
            .HasForeignKey(r => r.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        // MedicalRecordEntry
        modelBuilder.Entity<MedicalRecordEntry>()
            .HasOne(m => m.Patient)
            .WithMany()
            .HasForeignKey(m => m.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<MedicalRecordEntry>()
            .HasOne(m => m.AuthoringDoctor)
            .WithMany()
            .HasForeignKey(m => m.AuthoringDoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        // ChainOfCustodyLog ↔ Technician
        modelBuilder.Entity<ChainOfCustodyLog>()
            .HasOne(l => l.Technician)
            .WithMany(t => t.CustodyLogs)
            .HasForeignKey(l => l.TechnicianId)
            .OnDelete(DeleteBehavior.Restrict);

        // AvailabilitySchedule ↔ Doctor
        modelBuilder.Entity<AvailabilitySchedule>()
            .HasOne(s => s.Doctor)
            .WithMany(d => d.AvailabilitySchedules)
            .HasForeignKey(s => s.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Appointment ↔ Room (optional)
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.AssignedRoom)
            .WithMany()
            .HasForeignKey(a => a.RoomId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}