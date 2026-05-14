using IVF_Managment_Api.Dtos;

namespace IVF_Managment_Api.Services;

public interface IDonationBankService
{
    Task<DonationSampleResponseDto> CreateSampleAsync(CreateDonationSampleDto dto);
    Task<IEnumerable<DonationSampleResponseDto>> GetByDonorAsync(Guid donorId);
    Task<IEnumerable<DonationSampleResponseDto>> GetAssignableAsync();
    Task<DonationSampleResponseDto?> UpdateScreeningAsync(Guid sampleId, UpdateScreeningDto dto);
    Task<DonationSampleResponseDto?> UpdateQuantityAsync(Guid sampleId, int newQuantity);
}