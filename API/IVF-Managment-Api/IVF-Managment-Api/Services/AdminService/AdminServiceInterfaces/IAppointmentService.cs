using IVFClinic.DTOs.Appointment;
using IVFClinic.Models.Enums;

namespace IVFClinic.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<AppointmentResponseDto> CreateAppointmentAsync(AppointmentCreateDto dto, Guid adminId);
        Task<AppointmentResponseDto> RescheduleAppointmentAsync(Guid appointmentId, DateTime newStart, DateTime newEnd, Guid adminId);
        Task<bool> CancelAppointmentAsync(Guid appointmentId, string reason, Guid adminId);
        Task<bool> CheckScheduleConflictAsync(Guid doctorId, DateTime start, DateTime end, Guid? excludeAppointmentId = null);
        Task<IEnumerable<AppointmentResponseDto>> GetCalendarAsync(DateTime start, DateTime end, AppointmentFilters? filters);
        Task<AppointmentResponseDto?> GetAppointmentByIdAsync(Guid appointmentId);
    }

    public class AppointmentFilters
    {
        public Guid? DoctorId { get; set; }
        public Guid? PatientId { get; set; }
        public string? Department { get; set; }
        public AppointmentStatus? Status { get; set; }
    }
}
