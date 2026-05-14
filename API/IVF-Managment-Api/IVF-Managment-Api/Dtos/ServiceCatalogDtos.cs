using System.ComponentModel.DataAnnotations;

namespace IVF_Managment_Api.Dtos;

public class CreateClinicServiceDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }

    [Required]
    [MaxLength(100)]
    public string Category { get; set; }

    public string? Description { get; set; }

    [Required]
    public decimal Price { get; set; }
}

public class UpdateClinicServiceDto
{
    [MaxLength(200)]
    public string? Name { get; set; }

    [MaxLength(100)]
    public string? Category { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }
}

public class ClinicServiceResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}