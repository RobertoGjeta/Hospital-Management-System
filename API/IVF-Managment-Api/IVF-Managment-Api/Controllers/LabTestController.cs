using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IVF_Managment_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LabTestController : ControllerBase
{
    private readonly ILabTestService _service;

    public LabTestController(ILabTestService service) => _service = service;

    [HttpPost("order")]
    [Authorize(Roles = "Doctor")]
    public async Task<ActionResult<LabTestOrderResponseDto>> CreateOrder(CreateLabTestOrderDto dto)
    {
        var result = await _service.CreateOrderAsync(dto);
        return CreatedAtAction(nameof(GetReport), new { reportId = result.Id }, result);
    }

    [HttpGet("queue")]
    [Authorize(Roles = "LabTechnician")]
    public async Task<ActionResult<IEnumerable<LabTestOrderResponseDto>>> GetPendingQueue()
    {
        var result = await _service.GetPendingQueueAsync();
        return Ok(result);
    }

    [HttpPost("order/{orderId:guid}/result")]
    [Authorize(Roles = "LabTechnician")]
    public async Task<ActionResult<LabTestReportResponseDto>> UploadResult(Guid orderId, UploadLabTestResultDto dto)
    {
        try
        {
            var result = await _service.UploadResultAsync(orderId, dto);
            return CreatedAtAction(nameof(GetReport), new { reportId = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("report/{reportId:guid}/release")]
    [Authorize(Roles = "Doctor")]
    public async Task<ActionResult<LabTestReportResponseDto>> ReleaseToPatient(Guid reportId)
    {
        var result = await _service.ReleaseToPatientAsync(reportId);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpGet("patient/{patientId:guid}/released")]
    [Authorize(Roles = "Doctor,Patient")]
    public async Task<ActionResult<IEnumerable<LabTestReportResponseDto>>> GetReleasedForPatient(Guid patientId)
    {
        var result = await _service.GetReleasedForPatientAsync(patientId);
        return Ok(result);
    }

    [HttpGet("report/{reportId:guid}")]
    [Authorize(Roles = "Doctor,LabTechnician")]
    public async Task<ActionResult<LabTestReportResponseDto>> GetReport(Guid reportId)
    {
        var result = await _service.GetReportForDoctorAsync(reportId);
        if (result is null) return NotFound();
        return Ok(result);
    }
}