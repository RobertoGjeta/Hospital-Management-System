using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IVF_Managment_Api.Dtos;
using Microsoft.IdentityModel.Tokens;

namespace IVF_Managment_Api.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public AuthResponseDto? Authenticate(LoginDto dto)
    {
        var credential = UserCredentialStore.FindByUsernameOrEmail(dto.UsernameOrEmail);
        if (credential is null)
            return null;

        var hash = HashPassword(dto.Password);
        if (hash != credential.PasswordHash)
            return null;

        var expiresAt = DateTime.UtcNow.AddHours(
            double.Parse(_configuration["Jwt:ExpirationHours"] ?? "2"));

        var token = GenerateToken(credential, expiresAt);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = credential.Id,
            Email = credential.Email,
            Role = credential.Role.ToString()
        };
    }

    private string GenerateToken(UserCredentialStore.UserCredential credential, DateTime expiresAt)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, credential.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, credential.Email),
            new Claim(ClaimTypes.Role, credential.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string HashPassword(string password) =>
        Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(
            Encoding.UTF8.GetBytes(password)));
}