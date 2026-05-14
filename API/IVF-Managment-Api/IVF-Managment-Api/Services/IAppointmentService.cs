using IVF_Managment_Api.Dtos;
using IvfClinic.Models;

namespace IVF_Managment_Api.Services;

public interface IAppointmentService
{
    Task<AppointmentResponseDto> CreateAsync(CreateAppointmentDto dto);
    Task<AppointmentResponseDto?> RescheduleAsync(Guid id, DateTime newStart, DateTime newEnd, string? reason);
    Task<AppointmentResponseDto?> CancelAsync(Guid id, string reason);
    Task<IEnumerable<AppointmentResponseDto>> GetByPatientAsync(Guid patientId, AppointmentStatus? status = null);
    Task<IEnumerable<AppointmentResponseDto>> GetByDoctorAsync(Guid doctorId, DateTime from, DateTime to);
    Task<bool> CheckConflictAsync(Guid doctorId, DateTime start, DateTime end, Guid? excludeAppointmentId = null);
}