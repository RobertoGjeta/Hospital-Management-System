using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IVF_Managment_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponseDto>> Login(LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (result is null) return Unauthorized(new { message = "Invalid credentials." });
        return Ok(result);
    }

    [HttpPost("register/patient")]
    [AllowAnonymous]
    public async Task<ActionResult<PatientResponseDto>> RegisterPatient(CreatePatientDto dto)
    {
        var (result, error) = await _authService.RegisterPatientAsync(dto);
        if (error is not null) return Conflict(new { message = error });
        return StatusCode(201, result);
    }

    [HttpPost("register")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<RegisteredUserResponseDto>> AdminRegister(AdminRegisterDto dto)
    {
        var (result, error) = await _authService.AdminRegisterAsync(dto);
        if (error is not null)
        {
            if (error.Contains("already exists")) return Conflict(new { message = error });
            return BadRequest(new { message = error });
        }
        return StatusCode(201, result);
    }
}
