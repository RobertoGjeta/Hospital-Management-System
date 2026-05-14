using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IVF_Managment_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Doctor")]
public class MedicalRecordController : ControllerBase
{
    private readonly IMedicalRecordService _service;

    public MedicalRecordController(IMedicalRecordService service) => _service = service;

    [HttpPost]
    public async Task<ActionResult<MedicalRecordEntryResponseDto>> AddEntry(CreateMedicalRecordEntryDto dto)
    {
        var result = await _service.AddEntryAsync(dto);
        return CreatedAtAction(nameof(GetByPatient), new { patientId = result.PatientId }, result);
    }

    [HttpGet("patient/{patientId:guid}")]
    [Authorize(Roles = "Doctor,Patient")]
    public async Task<ActionResult<IEnumerable<MedicalRecordEntryResponseDto>>> GetByPatient(Guid patientId)
    {
        var result = await _service.GetByPatientAsync(patientId);
        return Ok(result);
    }
}