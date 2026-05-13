using IVF_Managment_Api.Dtos;

namespace IVF_Managment_Api.Services;

public interface IAuthService
{
    AuthResponseDto? Authenticate(LoginDto dto);
}