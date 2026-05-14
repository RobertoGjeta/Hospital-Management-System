using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IVF_Managment_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmbryoController : ControllerBase
{
    private readonly IEmbryoService _service;

    public EmbryoController(IEmbryoService service) => _service = service;

    [HttpPost]
    [Authorize(Roles = "Doctor,LabTechnician")]
    public async Task<ActionResult<EmbryoResponseDto>> Create(CreateEmbryoDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Doctor,LabTechnician")]
    public async Task<ActionResult<EmbryoResponseDto>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpGet("cycle/{cycleId:guid}")]
    [Authorize(Roles = "Doctor,LabTechnician")]
    public async Task<ActionResult<IEnumerable<EmbryoResponseDto>>> GetByCycle(Guid cycleId)
    {
        var result = await _service.GetByCycleAsync(cycleId);
        return Ok(result);
    }

    [HttpPost("development")]
    [Authorize(Roles = "LabTechnician")]
    public async Task<ActionResult<EmbryoDevelopmentEntryResponseDto>> AddDevelopmentEntry(CreateEmbryoDevelopmentEntryDto dto)
    {
        var result = await _service.AddDevelopmentEntryAsync(dto);
        return CreatedAtAction(nameof(GetDevelopmentEntries), new { embryoId = result.EmbryoId }, result);
    }

    [HttpGet("{embryoId:guid}/development")]
    [Authorize(Roles = "Doctor,LabTechnician")]
    public async Task<ActionResult<IEnumerable<EmbryoDevelopmentEntryResponseDto>>> GetDevelopmentEntries(Guid embryoId)
    {
        var result = await _service.GetDevelopmentEntriesAsync(embryoId);
        return Ok(result);
    }

    [HttpPost("cryopreservation")]
    [Authorize(Roles = "LabTechnician")]
    public async Task<ActionResult<EmbryoCryopreservationResponseDto>> RecordCryopreservation(CreateEmbryoCryopreservationDto dto)
    {
        try
        {
            var result = await _service.RecordCryopreservationAsync(dto);
            return Created(string.Empty, result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("instruction")]
    [Authorize(Roles = "Doctor")]
    public async Task<ActionResult<EmbryoClinicalInstructionResponseDto>> AddInstruction(CreateEmbryoClinicalInstructionDto dto)
    {
        try
        {
            var result = await _service.AddInstructionAsync(dto);
            return Created(string.Empty, result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}