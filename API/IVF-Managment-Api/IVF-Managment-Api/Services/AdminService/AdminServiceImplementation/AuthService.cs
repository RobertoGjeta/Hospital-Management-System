
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using IVFClinic.Data;
using IVFClinic.DTOs.Auth;
using IVFClinic.Models;
using IVFClinic.Models.Enums;
using IVFClinic.Services.Interfaces;

namespace IVFClinic.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        private readonly IAuditService _audit;

        private const int MaxFailedAttempts = 5;
        private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(30);
        private static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromHours(8);
        private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

        public AuthService(AppDbContext db, IConfiguration config, IAuditService audit)
        {
            _db = db;
            _config = config;
            _audit = audit;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto, string? ipAddress = null)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null)
            {
                return null; // do not reveal whether username exists
            }

            // Check lockout
            if (user.AccountLockedUntil.HasValue && user.AccountLockedUntil > DateTime.UtcNow)
            {
                await _audit.LogActionAsync(user.Id, AuditAction.LoginFailed, "User",
                    user.Id.ToString(), description: "Login attempted while locked", ipAddress: ipAddress);
                return null;
            }

            // Verify password (using BCrypt)
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= MaxFailedAttempts)
                {
                    user.AccountLockedUntil = DateTime.UtcNow.Add(LockoutDuration);
                }
                await _db.SaveChangesAsync();

                await _audit.LogActionAsync(user.Id, AuditAction.LoginFailed, "User",
                    user.Id.ToString(),
                    description: $"Failed login attempt {user.FailedLoginAttempts}",
                    ipAddress: ipAddress);
                return null;
            }

            if (!user.IsActive)
            {
                return null;
            }

            // Successful login: reset counters
            user.FailedLoginAttempts = 0;
            user.AccountLockedUntil = null;
            user.LastLoginAt = DateTime.UtcNow;

            var (accessToken, expires) = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.Add(RefreshTokenLifetime);
            await _db.SaveChangesAsync();

            await _audit.LogActionAsync(user.Id, AuditAction.Login, "User",
                user.Id.ToString(), description: "Successful login", ipAddress: ipAddress);

            return new AuthResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                Role = user.Role,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expires
            };
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }

            var (accessToken, expires) = GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.Add(RefreshTokenLifetime);
            await _db.SaveChangesAsync();

            return new AuthResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                Role = user.Role,
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = expires
            };
        }

        public async Task LogoutAsync(Guid userId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _db.SaveChangesAsync();
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            {
                return false;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.PasswordChangedAt = DateTime.UtcNow;
            user.RefreshToken = null; // invalidate existing sessions
            await _db.SaveChangesAsync();

            await _audit.LogActionAsync(userId, AuditAction.PasswordChange, "User",
                userId.ToString(), description: "Password changed by user");
            return true;
        }

        public async Task<bool> UnlockAccountAsync(Guid userId, Guid performedByAdminId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return false;

            user.FailedLoginAttempts = 0;
            user.AccountLockedUntil = null;
            await _db.SaveChangesAsync();

            await _audit.LogActionAsync(performedByAdminId, AuditAction.Update, "User",
                userId.ToString(), description: "Account unlocked by administrator");
            return true;
        }

        private (string token, DateTime expiresAt) GenerateAccessToken(User user)
        {
            var secret = _config["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret not configured");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.Add(AccessTokenLifetime);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("fullName", user.FullName)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds);

            return (new JwtSecurityTokenHandler().WriteToken(token), expires);
        }

        private static string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
