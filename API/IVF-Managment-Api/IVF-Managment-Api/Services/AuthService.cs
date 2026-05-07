using IVF_Managment_Api.Data;
using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Dtos.Auth;
using Microsoft.EntityFrameworkCore;

namespace IVF_Managment_Api.Services;

public class AuthService : IAuthService
{
    private readonly IvfDbContext _db;
    private readonly IJwtService _jwt;

    public AuthService(IvfDbContext db, IJwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
    {
        var identifier = dto.UsernameOrEmail.Trim().ToLowerInvariant();

        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u =>
                u.Email.ToLower() == identifier ||
                u.Username.ToLower() == identifier);

        if (user is null || !user.IsActive) return null;

        var hash = HashPassword(dto.Password);
        if (user.PasswordHash != hash) return null;

        var token = _jwt.GenerateToken(user);

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAt = _jwt.GetExpiration(),
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.ToString(),
            FullName = $"{user.FirstName} {user.LastName}"
        };
    }

    private static string HashPassword(string password) =>
        Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(password)));
}
