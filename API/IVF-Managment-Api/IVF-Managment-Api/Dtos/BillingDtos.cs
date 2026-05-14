using System.ComponentModel.DataAnnotations;
using IvfClinic.Models;

namespace IVF_Managment_Api.Dtos;

public class CreateBillDto
{
    [Required]
    public Guid PatientId { get; set; }

    [Required]
    public Guid GeneratedByAdminId { get; set; }

    public decimal Tax { get; set; }
    public decimal InsuranceDeduction { get; set; }
    public decimal Discount { get; set; }
    public string? DiscountReason { get; set; }
}

public class CreateBillLineItemDto
{
    [Required]
    public Guid BillId { get; set; }

    public Guid? ServiceId { get; set; }

    [Required]
    [MaxLength(300)]
    public string Description { get; set; }

    [Required]
    public decimal Amount { get; set; }
}

public class CreatePaymentDto
{
    [Required]
    public Guid BillId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public PaymentMethod Method { get; set; }

    [Required]
    public Guid RecordedByUserId { get; set; }
}

public class BillResponseDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string InvoiceNumber { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal InsuranceDeduction { get; set; }
    public decimal Discount { get; set; }
    public string? DiscountReason { get; set; }
    public decimal TotalDue { get; set; }
    public BillStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<BillLineItemResponseDto> LineItems { get; set; } = new();
}

public class BillLineItemResponseDto
{
    public Guid Id { get; set; }
    public Guid? ServiceId { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
}

public class PaymentResponseDto
{
    public Guid Id { get; set; }
    public Guid BillId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public Guid RecordedByUserId { get; set; }
    public DateTime PaymentDate { get; set; }
}