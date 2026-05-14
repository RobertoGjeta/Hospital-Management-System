using System.ComponentModel.DataAnnotations;
using IvfClinic.Models;

namespace IVF_Managment_Api.Dtos;

public class CreateSampleCustodyEventDto
{
    [Required]
    [MaxLength(100)]
    public string SampleIdentifier { get; set; }

    [Required]
    public CustodyEventType EventType { get; set; }

    [Required]
    public Guid TechnicianId { get; set; }

    public string? Recipient { get; set; }
    public string? ReasonCode { get; set; }
    public string? Notes { get; set; }
}

public class SampleCustodyEventResponseDto
{
    public Guid Id { get; set; }
    public string SampleIdentifier { get; set; }
    public CustodyEventType EventType { get; set; }
    public Guid TechnicianId { get; set; }
    public string? Recipient { get; set; }
    public string? ReasonCode { get; set; }
    public string? Notes { get; set; }
    public DateTime Timestamp { get; set; }
}