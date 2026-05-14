using IVF_Managment_Api.Dtos;

namespace IVF_Managment_Api.Services;

public interface ISampleCustodyService
{
    Task<SampleCustodyEventResponseDto> RecordEventAsync(CreateSampleCustodyEventDto dto);
    Task<IEnumerable<SampleCustodyEventResponseDto>> GetBySampleAsync(string sampleIdentifier);
}