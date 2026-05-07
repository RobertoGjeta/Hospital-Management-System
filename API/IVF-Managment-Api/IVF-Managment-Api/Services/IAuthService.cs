using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Dtos.Auth;

namespace IVF_Managment_Api.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginDto dto);
}
