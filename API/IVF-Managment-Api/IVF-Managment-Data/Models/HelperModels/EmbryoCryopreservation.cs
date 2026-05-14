using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IVF_Managment_Api.Models.HelperModels;

[Table("EmbryoCryopreservations")]
public class EmbryoCryopreservation
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid EmbryoId { get; set; }
    public virtual Embryo Embryo { get; set; }

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

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}