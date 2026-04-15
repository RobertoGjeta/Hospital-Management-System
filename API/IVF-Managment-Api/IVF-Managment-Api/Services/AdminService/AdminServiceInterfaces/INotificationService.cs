namespace IVFClinic.Services.Interfaces
{
    public enum NotificationChannel { Email, Sms, Both }

    public interface INotificationService
    {
        Task SendWelcomeNotificationAsync(Guid patientId, string username, string tempPassword);
        Task SendAppointmentCreatedAsync(Guid appointmentId);
        Task SendAppointmentRescheduledAsync(Guid appointmentId);
        Task SendAppointmentCancelledAsync(Guid appointmentId, string reason);
        Task SendBillNotificationAsync(Guid billId);
        Task SendPaymentReceiptAsync(Guid paymentId);
        Task SendDoctorCredentialsAsync(Guid doctorId, string username, string tempPassword);
        Task SendRoomAssignedAsync(Guid appointmentId, Guid roomId);
    }
}
