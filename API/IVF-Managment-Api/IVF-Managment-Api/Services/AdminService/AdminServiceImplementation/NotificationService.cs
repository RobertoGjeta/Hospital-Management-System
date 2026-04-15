using Microsoft.EntityFrameworkCore;
using IVFClinic.Data;
using IVFClinic.Services.Interfaces;

namespace IVFClinic.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(AppDbContext db, ILogger<NotificationService> logger)
        {
            _db = db;
            _logger = logger;
        }

        // NOTE: In production, integrate with SendGrid, Mailgun, Twilio, etc.
        // For now, these methods queue and log notifications.

        public async Task SendWelcomeNotificationAsync(Guid patientId, string username, string tempPassword)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            if (patient == null) return;

            _logger.LogInformation(
                "Welcome notification queued for patient {PatientId} ({Email}). Username: {Username}",
                patientId, patient.Email, username);

            // TODO: integrate with email/SMS provider
            await Task.CompletedTask;
        }

        public async Task SendAppointmentCreatedAsync(Guid appointmentId)
        {
            var appt = await _db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);
            if (appt == null) return;

            _logger.LogInformation(
                "Appointment confirmation queued for {PatientEmail} and {DoctorEmail}",
                appt.Patient.Email, appt.Doctor.Email);

            await Task.CompletedTask;
        }

        public async Task SendAppointmentRescheduledAsync(Guid appointmentId)
        {
            _logger.LogInformation("Reschedule notification queued for appointment {Id}", appointmentId);
            await Task.CompletedTask;
        }

        public async Task SendAppointmentCancelledAsync(Guid appointmentId, string reason)
        {
            _logger.LogInformation("Cancellation notification queued for appointment {Id}: {Reason}", appointmentId, reason);
            await Task.CompletedTask;
        }

        public async Task SendBillNotificationAsync(Guid billId)
        {
            _logger.LogInformation("Bill notification queued for bill {Id}", billId);
            await Task.CompletedTask;
        }

        public async Task SendPaymentReceiptAsync(Guid paymentId)
        {
            _logger.LogInformation("Payment receipt queued for payment {Id}", paymentId);
            await Task.CompletedTask;
        }

        public async Task SendDoctorCredentialsAsync(Guid doctorId, string username, string tempPassword)
        {
            _logger.LogInformation("Doctor credentials queued for doctor {Id}", doctorId);
            await Task.CompletedTask;
        }

        public async Task SendRoomAssignedAsync(Guid appointmentId, Guid roomId)
        {
            _logger.LogInformation("Room assignment notification queued for appointment {Id}", appointmentId);
            await Task.CompletedTask;
        }
    }
}
