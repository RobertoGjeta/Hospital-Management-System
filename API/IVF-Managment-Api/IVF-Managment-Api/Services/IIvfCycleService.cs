using IVF_Managment_Api.Dtos;

namespace IVF_Managment_Api.Services;

public interface IIvfCycleService
{
    Task<IvfCycleResponseDto> CreateAsync(CreateIvfCycleDto dto);
    Task<IEnumerable<IvfCycleResponseDto>> GetByPatientAsync(Guid patientId);
    Task<IvfCycleResponseDto?> AdvancePhaseAsync(Guid cycleId, Guid doctorId, string? justification);
}