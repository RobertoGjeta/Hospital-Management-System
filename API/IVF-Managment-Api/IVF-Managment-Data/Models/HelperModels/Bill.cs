using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IVF_Managment_Api.Models;
using IvfClinic.Models;

namespace IVF_Managment_Api.Models.HelperModels;

[Table("Bills")]
public class Bill
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid PatientId { get; set; }
    public virtual Patient Patient { get; set; }

    [Required]
    public Guid GeneratedByAdminId { get; set; }

    [Required]
    [MaxLength(50)]
    public string InvoiceNumber { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Subtotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal InsuranceDeduction { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; }
    public string DiscountReason { get; set; } // Mandatory if discount applied [cite: 358]

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalDue { get; set; }

    public BillStatus Status { get; set; } = BillStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Payment> Payments { get; set; }
}

[Table("Payments")]
public class Payment
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid BillId { get; set; }
    public virtual Bill Bill { get; set; }

    [Required]
    public Guid RecordedByAdminId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    public PaymentMethod Method { get; set; }

    public string TransactionReference { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
}