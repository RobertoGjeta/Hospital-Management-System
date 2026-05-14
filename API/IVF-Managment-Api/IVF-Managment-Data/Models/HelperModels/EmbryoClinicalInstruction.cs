using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IvfClinic.Models;

namespace IVF_Managment_Api.Models.HelperModels;

[Table("EmbryoClinicalInstructions")]
public class EmbryoClinicalInstruction
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid EmbryoId { get; set; }
    public virtual Embryo Embryo { get; set; }

    [Required]
    public Guid DoctorId { get; set; }

    [Required]
    public EmbryoInstructionType Type { get; set; }

    [Required]
    public string Rationale { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}