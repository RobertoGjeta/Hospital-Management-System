using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IVF_Managment_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Doctor")]
public class IvfCycleController : ControllerBase
{
    private readonly IIvfCycleService _service;

    public IvfCycleController(IIvfCycleService service) => _service = service;

    [HttpPost]
    public async Task<ActionResult<IvfCycleResponseDto>> Create(CreateIvfCycleDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetByPatient), new { patientId = result.PatientId }, result);
    }

    [HttpGet("patient/{patientId:guid}")]
    [Authorize(Roles = "Doctor,Patient")]
    public async Task<ActionResult<IEnumerable<IvfCycleResponseDto>>> GetByPatient(Guid patientId)
    {
        var result = await _service.GetByPatientAsync(patientId);
        return Ok(result);
    }

    [HttpPut("{cycleId:guid}/advance")]
    public async Task<ActionResult<IvfCycleResponseDto>> AdvancePhase(Guid cycleId, AdvancePhaseDto dto, [FromQuery] Guid doctorId)
    {
        try
        {
            var result = await _service.AdvancePhaseAsync(cycleId, doctorId, dto.Justification);
            if (result is null) return NotFound();
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}