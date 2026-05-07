using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Dtos.Auth;
using IVF_Managment_Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace IVF_Managment_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (result is null)
            return Unauthorized(new { message = "Invalid credentials or account is inactive." });
        return Ok(result);
    }
}
