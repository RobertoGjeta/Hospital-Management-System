using IVF_Managment_Api.Models.BaseModel;

namespace IVF_Managment_Api.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    DateTime GetExpiration();
}
