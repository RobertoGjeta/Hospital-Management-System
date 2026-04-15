using System.ComponentModel.DataAnnotations;

namespace IVF_Managment_Api.Models.BaseModel;

public class User
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; }
    
    [Required]
    [EmailAddress]
    public string Username { get; set; } 
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } 

    [Required]
    public string PasswordHash { get; set; }
    
    [Phone]
    [MaxLength(20)]
    public string PhoneNumber { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; } = true;
    
}