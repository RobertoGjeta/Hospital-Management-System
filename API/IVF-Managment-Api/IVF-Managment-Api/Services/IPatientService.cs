using IVF_Managment_Api.Dtos;

namespace IVF_Managment_Api.Services;

public interface IPatientService
{
    Task<IEnumerable<PatientResponseDto>> GetAllAsync();
    Task<PatientResponseDto?> GetByIdAsync(Guid id);
    Task<PatientResponseDto> CreateAsync(CreatePatientDto dto);
    Task<PatientResponseDto?> UpdateAsync(Guid id, UpdatePatientDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<PatientResponseDto>> SearchAsync(PatientSearchFilter filter);
    Task<IReadOnlyList<Guid>> DetectDuplicatesAsync(string nationalId, string firstName, string lastName, DateTime dob);
    Task<IEnumerable<PatientResponseDto>> GetAssignedToDoctorAsync(Guid doctorId);
    Task<PatientResponseDto?> UpdateContactInfoAsync(Guid patientId, UpdateContactDto dto);
}