using IVFClinic.DTOs.Doctor;
using IVFClinic.Models;

namespace IVFClinic.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<DoctorResponseDto> AddDoctorAsync(DoctorCreateDto dto, Guid adminId);
        Task<DoctorResponseDto> UpdateDoctorAsync(Guid doctorId, DoctorUpdateDto dto, Guid adminId);
        Task<bool> DeactivateDoctorAsync(Guid doctorId, Guid adminId, bool reassignAppointments);
        Task<bool> ReactivateDoctorAsync(Guid doctorId, Guid adminId);
        Task<DoctorResponseDto?> GetDoctorByIdAsync(Guid doctorId);
        Task<IEnumerable<DoctorResponseDto>> GetAllDoctorsAsync(bool includeInactive = false);
        Task<IEnumerable<Appointment>> GetFutureAppointmentsForDoctorAsync(Guid doctorId);
    }
}
