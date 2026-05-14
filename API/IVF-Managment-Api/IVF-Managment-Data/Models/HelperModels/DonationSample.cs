using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IvfClinic.Models;

namespace IVF_Managment_Api.Models.HelperModels;

[Table("DonationSamples")]
public class DonationSample
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid DonorId { get; set; }

    [Required]
    public DonationSampleType Type { get; set; }

    public int Quantity { get; set; }

    [Required]
    public DateTime CollectionDate { get; set; }

    [Required]
    public DateTime ExpiryDate { get; set; }

    [Required]
    [MaxLength(200)]
    public string StorageLocation { get; set; }

    [Required]
    public ScreeningStatus ScreeningStatus { get; set; } = ScreeningStatus.Pending;

    public string? ScreeningReason { get; set; }

    public bool IsAssignable { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}