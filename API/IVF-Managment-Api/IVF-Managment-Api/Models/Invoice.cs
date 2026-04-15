using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IVFClinic.Models.Enums;

namespace IVFClinic.Models
{
    public class Invoice
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(30)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        public Guid PatientId { get; set; }
        [ForeignKey(nameof(PatientId))]
        public Patient? Patient { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Tax { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal InsuranceCoverage { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }

        [MaxLength(500)]
        public string? DiscountReason { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalDue { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountPaid { get; set; } = 0;

        [Required]
        public BillStatus Status { get; set; } = BillStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DueDate { get; set; }

        public Guid CreatedById { get; set; }
        [ForeignKey(nameof(CreatedById))]
        public Admin? CreatedBy { get; set; }

        // Navigation properties
        public ICollection<InvoiceItem>? Items { get; set; }
        public ICollection<Payment>? Payments { get; set; }
    }

    public class InvoiceItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid InvoiceId { get; set; }
        [ForeignKey(nameof(InvoiceId))]
        public Invoice? Invoice { get; set; }

        public Guid? ServiceId { get; set; }
        [ForeignKey(nameof(ServiceId))]
        public ClinicService? Service { get; set; }

        [Required, MaxLength(300)]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotal { get; set; }
    }
}
