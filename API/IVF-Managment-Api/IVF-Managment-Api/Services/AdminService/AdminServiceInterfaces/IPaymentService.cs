using IVFClinic.DTOs.Payment;

namespace IVFClinic.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<OutstandingBillDto>> GetOutstandingBillsAsync(Guid patientId);
        Task<PaymentResponseDto> RecordPaymentAsync(PaymentCreateDto dto, Guid adminId);
        Task<PaymentResponseDto> IssueRefundAsync(Guid paymentId, decimal amount, string reason, Guid adminId);
        Task<PaymentResponseDto?> GetPaymentByIdAsync(Guid paymentId);
        Task<ReceiptDto> GenerateReceiptAsync(Guid paymentId);
    }
}
