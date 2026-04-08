using System.ComponentModel.DataAnnotations;

namespace IVF_Managment_Api.Dtos;

public class LoginDto
{
    [Required]
    [StringLength(256)]
    public string UsernameOrEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(256)]
    public string Password { get; set; } = string.Empty;
}
