using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Exceptions;
using IVF_Managment_Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IVF_Managment_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator,Doctor")]
public class DoctorController : ControllerBase
{
    private readonly IDoctorService _service;

    public DoctorController(IDoctorService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DoctorResponseDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DoctorResponseDto>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<DoctorResponseDto>> Create(CreateDoctorDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<DoctorResponseDto>> Update(Guid id, UpdateDoctorDto dto)
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

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<DoctorResponseDto>>> GetActive()
    {
        var result = await _service.GetActiveAsync();
        return Ok(result);
    }

    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        try
        {
            await _service.DeactivateAsync(id);
            return NoContent();
        }
        catch (HasFutureAppointmentsException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{doctorId:guid}/availability")]
    public async Task<ActionResult<IEnumerable<DoctorAvailabilityResponseDto>>> GetAvailability(Guid doctorId, [FromQuery] DateOnly from, [FromQuery] DateOnly to)
    {
        var result = await _service.GetAvailabilityAsync(doctorId, from, to);
        return Ok(result);
    }

    [HttpPut("{doctorId:guid}/availability")]
    public async Task<IActionResult> SetAvailability(Guid doctorId, List<AvailabilitySlotDto> slots)
    {
        await _service.SetAvailabilityAsync(doctorId, slots);
        return NoContent();
    }
}