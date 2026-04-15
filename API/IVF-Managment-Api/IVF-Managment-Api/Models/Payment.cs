using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IVFClinic.Models.Enums;

namespace IVFClinic.Models
{
    public class Payment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid InvoiceId { get; set; }
        [ForeignKey(nameof(InvoiceId))]
        public Invoice? Invoice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMethod Method { get; set; }

        [MaxLength(100)]
        public string? ReferenceNumber { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime PaidAt { get; set; } = DateTime.UtcNow;

        public bool IsRefund { get; set; } = false;

        public Guid? OriginalPaymentId { get; set; }
        [ForeignKey(nameof(OriginalPaymentId))]
        public Payment? OriginalPayment { get; set; }

        public Guid RecordedById { get; set; }
        [ForeignKey(nameof(RecordedById))]
        public Admin? RecordedBy { get; set; }
    }
}
