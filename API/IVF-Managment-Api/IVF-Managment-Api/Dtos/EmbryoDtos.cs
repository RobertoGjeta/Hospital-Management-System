using System.ComponentModel.DataAnnotations;
using IvfClinic.Models;

namespace IVF_Managment_Api.Dtos;

public class CreateEmbryoDto
{
    [Required]
    public Guid IvfCycleId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Identifier { get; set; }
}

public class EmbryoResponseDto
{
    public Guid Id { get; set; }
    public Guid IvfCycleId { get; set; }
    public string Identifier { get; set; }
    public EmbryoStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateEmbryoDevelopmentEntryDto
{
    [Required]
    public Guid EmbryoId { get; set; }

    [Required]
    public Guid TechnicianId { get; set; }

    [Required]
    public int DevelopmentDay { get; set; }

    [Required]
    public int CellCount { get; set; }

    public decimal FragmentationPct { get; set; }

    [Required]
    [MaxLength(20)]
    public string MorphologyGrade { get; set; }

    public string? Notes { get; set; }

    [MaxLength(500)]
    public string? MicroscopyImagePath { get; set; }
}

public class EmbryoDevelopmentEntryResponseDto
{
    public Guid Id { get; set; }
    public Guid EmbryoId { get; set; }
    public Guid TechnicianId { get; set; }
    public int DevelopmentDay { get; set; }
    public int CellCount { get; set; }
    public decimal FragmentationPct { get; set; }
    public string MorphologyGrade { get; set; }
    public string? Notes { get; set; }
    public string? MicroscopyImagePath { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateEmbryoCryopreservationDto
{
    [Required]
    public Guid EmbryoId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Tank { get; set; }

    [Required]
    [MaxLength(50)]
    public string Cane { get; set; }

    [Required]
    [MaxLength(50)]
    public string StrawPosition { get; set; }

    [Required]
    public DateTime FreezingDate { get; set; }

    [Required]
    [MaxLength(100)]
    public string VitrificationMethod { get; set; }

    [Required]
    public Guid TechnicianId { get; set; }
}

public class EmbryoCryopreservationResponseDto
{
    public Guid Id { get; set; }
    public Guid EmbryoId { get; set; }
    public string Tank { get; set; }
    public string Cane { get; set; }
    public string StrawPosition { get; set; }
    public DateTime FreezingDate { get; set; }
    public string VitrificationMethod { get; set; }
    public Guid TechnicianId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateEmbryoClinicalInstructionDto
{
    [Required]
    public Guid EmbryoId { get; set; }

    [Required]
    public Guid DoctorId { get; set; }

    [Required]
    public EmbryoInstructionType Type { get; set; }

    [Required]
    public string Rationale { get; set; }

    public string? Password { get; set; }
}

public class EmbryoClinicalInstructionResponseDto
{
    public Guid Id { get; set; }
    public Guid EmbryoId { get; set; }
    public Guid DoctorId { get; set; }
    public EmbryoInstructionType Type { get; set; }
    public string Rationale { get; set; }
    public DateTime CreatedAt { get; set; }
}