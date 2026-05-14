using System.ComponentModel.DataAnnotations;
using IvfClinic.Models;

namespace IVF_Managment_Api.Dtos;

public class CreateDonationSampleDto
{
    [Required]
    public Guid DonorId { get; set; }

    [Required]
    public DonationSampleType Type { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public DateTime CollectionDate { get; set; }

    [Required]
    public DateTime ExpiryDate { get; set; }

    [Required]
    [MaxLength(200)]
    public string StorageLocation { get; set; }
}

public class UpdateScreeningDto
{
    [Required]
    public ScreeningStatus Status { get; set; }

    public string? Reason { get; set; }
}

public class DonationSampleResponseDto
{
    public Guid Id { get; set; }
    public Guid DonorId { get; set; }
    public DonationSampleType Type { get; set; }
    public int Quantity { get; set; }
    public DateTime CollectionDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string StorageLocation { get; set; }
    public ScreeningStatus ScreeningStatus { get; set; }
    public string? ScreeningReason { get; set; }
    public bool IsAssignable { get; set; }
    public DateTime CreatedAt { get; set; }
}