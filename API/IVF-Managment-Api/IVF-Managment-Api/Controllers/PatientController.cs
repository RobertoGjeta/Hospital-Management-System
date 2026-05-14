using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Exceptions;
using IVF_Managment_Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IVF_Managment_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator,Doctor,Patient")]
public class PatientController : ControllerBase
{
    private readonly IPatientService _service;

    public PatientController(IPatientService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientResponseDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PatientResponseDto>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<PatientResponseDto>> Create(CreatePatientDto dto)
    {
        try
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (DuplicatePatientException ex)
        {
            return Conflict(new { message = ex.Message, existingIds = ex.ExistingPatientIds });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PatientResponseDto>> Update(Guid id, UpdatePatientDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<PatientResponseDto>>> Search([FromQuery] PatientSearchFilter filter)
    {
        var result = await _service.SearchAsync(filter);
        return Ok(result);
    }

    [HttpGet("doctor/{doctorId:guid}")]
    public async Task<ActionResult<IEnumerable<PatientResponseDto>>> GetAssignedToDoctor(Guid doctorId)
    {
        var result = await _service.GetAssignedToDoctorAsync(doctorId);
        return Ok(result);
    }

    [HttpPatch("{patientId:guid}/contact")]
    public async Task<ActionResult<PatientResponseDto>> UpdateContactInfo(Guid patientId, UpdateContactDto dto)
    {
        var result = await _service.UpdateContactInfoAsync(patientId, dto);
        if (result is null) return NotFound();
        return Ok(result);
    }
}