using IVF_Managment_Api.Dtos;

namespace IVF_Managment_Api.Services;

public interface ILabTestService
{
    Task<LabTestOrderResponseDto> CreateOrderAsync(CreateLabTestOrderDto dto);
    Task<IEnumerable<LabTestOrderResponseDto>> GetPendingQueueAsync();
    Task<LabTestReportResponseDto> UploadResultAsync(Guid orderId, UploadLabTestResultDto dto);
    Task<LabTestReportResponseDto?> ReleaseToPatientAsync(Guid reportId);
    Task<IEnumerable<LabTestReportResponseDto>> GetReleasedForPatientAsync(Guid patientId);
    Task<LabTestReportResponseDto?> GetReportForDoctorAsync(Guid reportId);
}