using IVF_Managment_Api.Dtos;

namespace IVF_Managment_Api.Services;

public interface IEmbryoService
{
    Task<EmbryoResponseDto> CreateAsync(CreateEmbryoDto dto);
    Task<IEnumerable<EmbryoResponseDto>> GetByCycleAsync(Guid cycleId);
    Task<EmbryoResponseDto?> GetByIdAsync(Guid id);
    Task<EmbryoDevelopmentEntryResponseDto> AddDevelopmentEntryAsync(CreateEmbryoDevelopmentEntryDto dto);
    Task<IEnumerable<EmbryoDevelopmentEntryResponseDto>> GetDevelopmentEntriesAsync(Guid embryoId);
    Task<EmbryoCryopreservationResponseDto> RecordCryopreservationAsync(CreateEmbryoCryopreservationDto dto);
    Task<EmbryoClinicalInstructionResponseDto> AddInstructionAsync(CreateEmbryoClinicalInstructionDto dto);
}