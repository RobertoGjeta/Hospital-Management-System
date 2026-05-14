using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Services;
using IvfClinic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IVF_Managment_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _service;

    public AppointmentController(IAppointmentService service) => _service = service;

    [HttpPost]
    public async Task<ActionResult<AppointmentResponseDto>> Create(CreateAppointmentDto dto)
    {
        try
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetByPatient), new { patientId = result.PatientId }, result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}/reschedule")]
    public async Task<ActionResult<AppointmentResponseDto>> Reschedule(Guid id, RescheduleAppointmentDto dto)
    {
        try
        {
            var result = await _service.RescheduleAsync(id, dto.NewStartsAt, dto.NewEndsAt, dto.Reason);
            if (result is null) return NotFound();
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<ActionResult<AppointmentResponseDto>> Cancel(Guid id, CancelAppointmentDto dto)
    {
        var result = await _service.CancelAsync(id, dto.Reason);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpGet("patient/{patientId:guid}")]
    public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetByPatient(Guid patientId, [FromQuery] AppointmentStatus? status)
    {
        var result = await _service.GetByPatientAsync(patientId, status);
        return Ok(result);
    }

    [HttpGet("doctor/{doctorId:guid}")]
    public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetByDoctor(Guid doctorId, [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await _service.GetByDoctorAsync(doctorId, from, to);
        return Ok(result);
    }
}