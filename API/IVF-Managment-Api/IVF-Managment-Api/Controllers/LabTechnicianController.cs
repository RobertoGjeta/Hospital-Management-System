using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Mappings;
using IVF_Managment_Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace IVF_Managment_Api.Controllers;

/// <summary>
/// HTTP API for lab technician registration, profiles, and credential checks.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LabTechnicianController : ControllerBase
{
    private readonly ILabTechnicianService _labTechnicianService;

    public LabTechnicianController(ILabTechnicianService labTechnicianService)
    {
        _labTechnicianService = labTechnicianService;
    }

    /// <summary>
    /// Returns all lab technicians, optionally restricted to active accounts.
    /// </summary>
    /// <param name="activeOnly">When true, only technicians with <c>IsActive == true</c> are returned.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LabTechnicianResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LabTechnicianResponseDto>>> GetAll(
        [FromQuery] bool activeOnly = false)
    {
        var technicians = activeOnly
            ? await _labTechnicianService.GetActiveAsync()
            : await _labTechnicianService.GetAllAsync();

        return Ok(technicians.Select(LabTechnicianMapper.ToResponse));
    }

    /// <summary>
    /// Gets a lab technician by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LabTechnicianResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LabTechnicianResponseDto>> GetById(Guid id)
    {
        var technician = await _labTechnicianService.GetByIdAsync(id);
        if (technician is null)
            return NotFound();

        return Ok(LabTechnicianMapper.ToResponse(technician));
    }

    /// <summary>
    /// Looks up a lab technician by exact username.
    /// </summary>
    [HttpGet("by-username/{username}")]
    [ProducesResponseType(typeof(LabTechnicianResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LabTechnicianResponseDto>> GetByUsername(string username)
    {
        var technician = await _labTechnicianService.GetByUsernameAsync(username);
        if (technician is null)
            return NotFound();

        return Ok(LabTechnicianMapper.ToResponse(technician));
    }

    /// <summary>
    /// Looks up a lab technician by exact email address.
    /// </summary>
    [HttpGet("by-email/{email}")]
    [ProducesResponseType(typeof(LabTechnicianResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LabTechnicianResponseDto>> GetByEmail(string email)
    {
        var technician = await _labTechnicianService.GetByEmailAsync(email);
        if (technician is null)
            return NotFound();

        return Ok(LabTechnicianMapper.ToResponse(technician));
    }

    /// <summary>
    /// Searches lab technicians by name, specialization, or employee ID.
    /// </summary>
    /// <param name="q">Case-insensitive search term applied against first name, last name, specialization, and employee ID.</param>
    /// <param name="activeOnly">When true, only active technicians are searched.</param>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<LabTechnicianResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<LabTechnicianResponseDto>>> Search(
        [FromQuery] string? q,
        [FromQuery] bool activeOnly = false)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { message = "Query parameter 'q' is required." });

        var technicians = activeOnly
            ? await _labTechnicianService.GetActiveAsync()
            : await _labTechnicianService.GetAllAsync();

        var term = q.Trim();
        var results = technicians.Where(t =>
            t.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
            t.LastName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
            (t.Specialization?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (t.EmployeeId?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false));

        return Ok(results.Select(LabTechnicianMapper.ToResponse));
    }

    /// <summary>
    /// Registers a new lab technician.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(LabTechnicianResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LabTechnicianResponseDto>> Create([FromBody] CreateLabTechnicianDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var entity = LabTechnicianMapper.ToEntity(dto);
            var created = await _labTechnicianService.CreateAsync(entity, dto.Password);
            var response = LabTechnicianMapper.ToResponse(created);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Updates profile fields for an existing lab technician.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(LabTechnicianResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LabTechnicianResponseDto>> Update(Guid id, [FromBody] UpdateLabTechnicianDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updated = await _labTechnicianService.UpdateAsync(id, LabTechnicianMapper.ToEntity(dto));
            if (updated is null)
                return NotFound();

            return Ok(LabTechnicianMapper.ToResponse(updated));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a lab technician.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _labTechnicianService.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Sets <c>IsActive</c> to false for the technician.
    /// </summary>
    [HttpPatch("{id:guid}/deactivate")]
    [ProducesResponseType(typeof(LabTechnicianResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LabTechnicianResponseDto>> Deactivate(Guid id)
    {
        var ok = await _labTechnicianService.DeactivateAsync(id);
        if (!ok)
            return NotFound();

        var technician = await _labTechnicianService.GetByIdAsync(id);
        return Ok(LabTechnicianMapper.ToResponse(technician!));
    }

    /// <summary>
    /// Sets <c>IsActive</c> to true and clears lockout counters where applicable.
    /// </summary>
    [HttpPatch("{id:guid}/activate")]
    [ProducesResponseType(typeof(LabTechnicianResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LabTechnicianResponseDto>> Activate(Guid id)
    {
        var ok = await _labTechnicianService.ActivateAsync(id);
        if (!ok)
            return NotFound();

        var technician = await _labTechnicianService.GetByIdAsync(id);
        return Ok(LabTechnicianMapper.ToResponse(technician!));
    }

    /// <summary>
    /// Changes the technician's password after verifying the current password.
    /// </summary>
    [HttpPost("{id:guid}/change-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _labTechnicianService.GetByIdAsync(id) is null)
            return NotFound();

        try
        {
            var changed = await _labTechnicianService.ChangePasswordAsync(id, dto.CurrentPassword, dto.NewPassword);
            if (!changed)
                return BadRequest(new { message = "Current password is incorrect." });

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Validates username/email and password. Returns the technician profile on success.
    /// Returns 401 with a reason hint when the account is locked versus invalid credentials.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LabTechnicianResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LabTechnicianResponseDto>> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var technician = await _labTechnicianService.ValidateCredentialsAsync(dto.UsernameOrEmail, dto.Password);
            if (technician is not null)
                return Ok(LabTechnicianMapper.ToResponse(technician));

            var existing = await _labTechnicianService.GetByUsernameAsync(dto.UsernameOrEmail)
                           ?? await _labTechnicianService.GetByEmailAsync(dto.UsernameOrEmail);

            if (existing is not null && !existing.IsActive)
                return Unauthorized(new { message = "Account is deactivated. Please contact an administrator." });

            if (existing?.AccountLockedUntil > DateTime.UtcNow)
                return Unauthorized(new { message = "Account is temporarily locked due to too many failed attempts." });

            return Unauthorized(new { message = "Invalid username/email or password." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
