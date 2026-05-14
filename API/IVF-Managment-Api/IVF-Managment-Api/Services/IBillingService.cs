using IVF_Managment_Api.Dtos;

namespace IVF_Managment_Api.Services;

public interface IBillingService
{
    Task<BillResponseDto> CreateBillAsync(CreateBillDto dto);
    Task<BillResponseDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<BillResponseDto>> GetByPatientAsync(Guid patientId);
    Task<BillLineItemResponseDto> AddLineItemAsync(CreateBillLineItemDto dto);
    Task<PaymentResponseDto> RecordPaymentAsync(CreatePaymentDto dto);
}