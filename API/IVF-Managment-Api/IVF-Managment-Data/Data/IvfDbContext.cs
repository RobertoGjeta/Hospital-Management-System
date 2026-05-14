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
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLogEntry> AuditLogEntries => Set<AuditLogEntry>();
    public DbSet<EmbryoCryopreservation> EmbryoCryopreservations => Set<EmbryoCryopreservation>();
    public DbSet<EmbryoClinicalInstruction> EmbryoClinicalInstructions => Set<EmbryoClinicalInstruction>();
    public DbSet<ClinicService> ClinicServices => Set<ClinicService>();
    public DbSet<BillLineItem> BillLineItems => Set<BillLineItem>();
    public DbSet<DonationSample> DonationSamples => Set<DonationSample>();

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
            .OnDelete(DeleteBehavior.NoAction);

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

        // Embryo ↔ IvfCycle
        modelBuilder.Entity<Embryo>()
            .HasOne(e => e.IvfCycle)
            .WithMany(c => c.Embryos)
            .HasForeignKey(e => e.IvfCycleId)
            .OnDelete(DeleteBehavior.Cascade);

        // EmbryoObservation ↔ Embryo + Technician
        modelBuilder.Entity<EmbryoObservation>()
            .HasOne(o => o.Embryo)
            .WithMany(e => e.Observations)
            .HasForeignKey(o => o.EmbryoId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<EmbryoObservation>()
            .HasOne(o => o.Technician)
            .WithMany(t => t.EmbryoObservations)
            .HasForeignKey(o => o.TechnicianId)
            .OnDelete(DeleteBehavior.Restrict);

        // EmbryoCryopreservation ↔ Embryo
        modelBuilder.Entity<EmbryoCryopreservation>()
            .HasOne(c => c.Embryo)
            .WithMany()
            .HasForeignKey(c => c.EmbryoId)
            .OnDelete(DeleteBehavior.Cascade);

        // EmbryoClinicalInstruction ↔ Embryo
        modelBuilder.Entity<EmbryoClinicalInstruction>()
            .HasOne(i => i.Embryo)
            .WithMany()
            .HasForeignKey(i => i.EmbryoId)
            .OnDelete(DeleteBehavior.Cascade);

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
            .OnDelete(DeleteBehavior.NoAction);

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

        // MedicalRecordEntry self-FK (amendment chain)
        modelBuilder.Entity<MedicalRecordEntry>()
            .HasOne(m => m.AmendsEntry)
            .WithMany()
            .HasForeignKey(m => m.AmendsEntryId)
            .OnDelete(DeleteBehavior.Restrict);

        // LabTestReport ↔ LabTestOrder
        modelBuilder.Entity<LabTestReport>()
            .HasOne(r => r.Order)
            .WithMany()
            .HasForeignKey(r => r.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Enum → string conversions
        modelBuilder.Entity<Appointment>()
            .Property(a => a.Status).HasConversion<string>();
        modelBuilder.Entity<Notification>()
            .Property(n => n.Type).HasConversion<string>();
        modelBuilder.Entity<Notification>()
            .Property(n => n.Channel).HasConversion<string>();
        modelBuilder.Entity<LabTestOrder>()
            .Property(o => o.Priority).HasConversion<string>();
        modelBuilder.Entity<LabTestOrder>()
            .Property(o => o.Status).HasConversion<string>();
        modelBuilder.Entity<MedicalRecordEntry>()
            .Property(m => m.EntryType).HasConversion<string>();
        modelBuilder.Entity<IvfCycle>()
            .Property(c => c.CurrentPhase).HasConversion<string>();
        modelBuilder.Entity<Embryo>()
            .Property(e => e.Status).HasConversion<string>();
        modelBuilder.Entity<EmbryoClinicalInstruction>()
            .Property(i => i.Type).HasConversion<string>();

        // BillLineItem ↔ Bill
        modelBuilder.Entity<BillLineItem>()
            .HasOne(li => li.Bill)
            .WithMany(b => b.LineItems)
            .HasForeignKey(li => li.BillId)
            .OnDelete(DeleteBehavior.Cascade);

        // ClinicService unique name within category
        modelBuilder.Entity<ClinicService>()
            .HasIndex(s => new { s.Name, s.Category })
            .IsUnique();

        // DonationSample enum conversions
        modelBuilder.Entity<DonationSample>()
            .Property(d => d.Type).HasConversion<string>();
        modelBuilder.Entity<DonationSample>()
            .Property(d => d.ScreeningStatus).HasConversion<string>();

        // ChainOfCustodyLog enum conversion
        modelBuilder.Entity<ChainOfCustodyLog>()
            .Property(l => l.EventType).HasConversion<string>();

        // Payment enum conversion
        modelBuilder.Entity<Payment>()
            .Property(p => p.Method).HasConversion<string>();
        modelBuilder.Entity<Bill>()
            .Property(b => b.Status).HasConversion<string>();
    }
}