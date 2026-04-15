using IVFClinic.DTOs.Patient;
using IVFClinic.Models;

namespace IVFClinic.Services.Interfaces
{
    public interface IPatientService
    {
        Task<PatientResponseDto> RegisterPatientAsync(PatientRegisterDto dto, Guid adminId, bool forceCreate = false);
        Task<Patient?> CheckDuplicateAsync(string nationalId, string firstName, string lastName, DateTime dob);
        Task<PagedResult<PatientResponseDto>> SearchPatientsAsync(string? searchTerm, PatientFilters? filters, int page, int pageSize);
        Task<PatientResponseDto?> GetPatientByIdAsync(Guid patientId);
        Task<PatientResponseDto> UpdatePatientAsync(Guid patientId, PatientUpdateDto dto, Guid adminId);
    }

    public class PatientFilters
    {
        public string? TreatmentType { get; set; }
        public DateTime? AppointmentDateFrom { get; set; }
        public DateTime? AppointmentDateTo { get; set; }
        public bool? IsActive { get; set; }
    }

    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
