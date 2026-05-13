namespace IVF_Managment_Api.Dtos;

public class TokenResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}