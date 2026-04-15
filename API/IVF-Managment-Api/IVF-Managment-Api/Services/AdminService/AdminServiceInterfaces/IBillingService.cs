using IVFClinic.DTOs.Billing;
using IVFClinic.Models.Enums;

namespace IVFClinic.Services.Interfaces
{
    public interface IBillingService
    {
        Task<IEnumerable<UnbilledServiceDto>> GetUnbilledServicesAsync(Guid patientId);
        Task<BillPreviewDto> PreviewBillAsync(Guid patientId, IEnumerable<Guid> serviceIds);
        Task<InvoiceResponseDto> GenerateBillAsync(BillCreateDto dto, Guid adminId);
        Task<InvoiceResponseDto> ApplyDiscountAsync(Guid invoiceId, decimal discount, string reason, Guid adminId);
        Task<InvoiceResponseDto?> GetBillByIdAsync(Guid invoiceId);
        Task<IEnumerable<InvoiceResponseDto>> GetPatientBillsAsync(Guid patientId, BillStatus? status = null);
    }
}
