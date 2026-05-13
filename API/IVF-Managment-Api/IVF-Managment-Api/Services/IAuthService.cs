using IVF_Managment_Api.Dtos;

namespace IVF_Managment_Api.Services;

public interface IAuthService
{
    Task<TokenResponseDto?> LoginAsync(LoginDto dto);
    Task<(PatientResponseDto? Result, string? Error)> RegisterPatientAsync(CreatePatientDto dto);
    Task<(RegisteredUserResponseDto? Result, string? Error)> AdminRegisterAsync(AdminRegisterDto dto);
}
