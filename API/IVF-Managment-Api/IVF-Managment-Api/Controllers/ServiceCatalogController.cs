using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IVF_Managment_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ServiceCatalogController : ControllerBase
{
    private readonly IServiceCatalogService _service;

    public ServiceCatalogController(IServiceCatalogService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClinicServiceResponseDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClinicServiceResponseDto>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ClinicServiceResponseDto>> Create(CreateClinicServiceDto dto)
    {
        try
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ClinicServiceResponseDto>> Update(Guid id, UpdateClinicServiceDto dto)
    {
        try
        {
            var result = await _service.UpdateAsync(id, dto);
            if (result is null) return NotFound();
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var deactivated = await _service.DeactivateAsync(id);
        if (!deactivated) return NotFound();
        return NoContent();
    }
}