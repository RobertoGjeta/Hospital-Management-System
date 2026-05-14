using IVF_Managment_Api.Dtos;

namespace IVF_Managment_Api.Services;

public interface IMedicalRecordService
{
    Task<MedicalRecordEntryResponseDto> AddEntryAsync(CreateMedicalRecordEntryDto dto);
    Task<IEnumerable<MedicalRecordEntryResponseDto>> GetByPatientAsync(Guid patientId);
}