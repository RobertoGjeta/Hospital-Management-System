using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IVF_Managment_Api.Models.HelperModels;

[Table("BillLineItems")]
public class BillLineItem
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid BillId { get; set; }
    public virtual Bill Bill { get; set; }

    public Guid? ServiceId { get; set; }

    [Required]
    [MaxLength(300)]
    public string Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
}