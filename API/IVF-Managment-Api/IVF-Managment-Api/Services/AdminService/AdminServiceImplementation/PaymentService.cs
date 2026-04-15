using Microsoft.EntityFrameworkCore;
using IVFClinic.Data;
using IVFClinic.DTOs.Payment;
using IVFClinic.Models;
using IVFClinic.Models.Enums;
using IVFClinic.Services.Interfaces;

namespace IVFClinic.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _db;
        private readonly IAuditService _audit;
        private readonly INotificationService _notif;

        public PaymentService(AppDbContext db, IAuditService audit, INotificationService notif)
        {
            _db = db;
            _audit = audit;
            _notif = notif;
        }

        public async Task<IEnumerable<OutstandingBillDto>> GetOutstandingBillsAsync(Guid patientId)
        {
            return await _db.Invoices
                .Where(i => i.PatientId == patientId
                    && (i.Status == BillStatus.Pending || i.Status == BillStatus.PartiallyPaid))
                .Select(i => new OutstandingBillDto
                {
                    InvoiceId = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    TotalDue = i.TotalDue,
                    AmountPaid = i.AmountPaid,
                    RemainingBalance = i.TotalDue - i.AmountPaid,
                    CreatedAt = i.CreatedAt,
                    Status = i.Status
                })
                .ToListAsync();
        }

        public async Task<PaymentResponseDto> RecordPaymentAsync(PaymentCreateDto dto, Guid adminId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var invoice = await _db.Invoices.FindAsync(dto.InvoiceId)
                    ?? throw new KeyNotFoundException("Invoice not found");

                var remainingBalance = invoice.TotalDue - invoice.AmountPaid;

                // Validate amount (FR_A12)
                if (dto.Amount <= 0)
                    throw new ArgumentException("Payment amount must be positive");

                if (dto.Amount > remainingBalance)
                    throw new InvalidOperationException(
                        $"Payment amount {dto.Amount} exceeds remaining balance {remainingBalance}");

                // Record the payment
                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    InvoiceId = dto.InvoiceId,
                    Amount = dto.Amount,
                    Method = dto.Method,
                    ReferenceNumber = dto.ReferenceNumber,
                    PaidAt = DateTime.UtcNow,
                    RecordedById = adminId
                };

                _db.Payments.Add(payment);

                // Update invoice status
                invoice.AmountPaid += dto.Amount;
                if (invoice.AmountPaid >= invoice.TotalDue)
                {
                    invoice.Status = BillStatus.Paid;
                }
                else
                {
                    invoice.Status = BillStatus.PartiallyPaid;
                }
                invoice.UpdatedAt = DateTime.UtcNow;

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                await _audit.LogActionAsync(adminId, AuditAction.RecordPayment, "Payment",
                    payment.Id.ToString(), newValues: dto);

                await _notif.SendPaymentReceiptAsync(payment.Id);

                return MapToDto(payment, invoice);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PaymentResponseDto> IssueRefundAsync(
            Guid paymentId, decimal amount, string reason, Guid adminId)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Refund reason is mandatory");

            var originalPayment = await _db.Payments
                .Include(p => p.Invoice)
                .FirstOrDefaultAsync(p => p.Id == paymentId)
                ?? throw new KeyNotFoundException("Payment not found");

            if (amount > originalPayment.Amount)
                throw new InvalidOperationException("Refund amount exceeds original payment");

            // Refunds are recorded as negative payments
            var refund = new Payment
            {
                Id = Guid.NewGuid(),
                InvoiceId = originalPayment.InvoiceId,
                Amount = -amount,
                Method = originalPayment.Method,
                ReferenceNumber = $"REFUND-{originalPayment.ReferenceNumber}",
                Notes = reason,
                PaidAt = DateTime.UtcNow,
                RecordedById = adminId,
                IsRefund = true,
                OriginalPaymentId = paymentId
            };

            _db.Payments.Add(refund);

            originalPayment.Invoice!.AmountPaid -= amount;
            if (originalPayment.Invoice.AmountPaid < originalPayment.Invoice.TotalDue)
            {
                originalPayment.Invoice.Status =
                    originalPayment.Invoice.AmountPaid > 0 ? BillStatus.PartiallyPaid : BillStatus.Pending;
            }

            await _db.SaveChangesAsync();

            await _audit.LogActionAsync(adminId, AuditAction.Update, "Payment",
                refund.Id.ToString(), description: $"Refund issued: {reason}");

            return MapToDto(refund, originalPayment.Invoice);
        }

        public async Task<PaymentResponseDto?> GetPaymentByIdAsync(Guid paymentId)
        {
            var payment = await _db.Payments
                .Include(p => p.Invoice)
                .FirstOrDefaultAsync(p => p.Id == paymentId);
            return payment == null ? null : MapToDto(payment, payment.Invoice!);
        }

        public async Task<ReceiptDto> GenerateReceiptAsync(Guid paymentId)
        {
            var payment = await _db.Payments
                .Include(p => p.Invoice)
                    .ThenInclude(i => i!.Patient)
                .FirstOrDefaultAsync(p => p.Id == paymentId)
                ?? throw new KeyNotFoundException("Payment not found");

            return new ReceiptDto
            {
                ReceiptNumber = $"RCP-{payment.Id.ToString("N")[..8].ToUpper()}",
                PaymentId = payment.Id,
                InvoiceNumber = payment.Invoice!.InvoiceNumber,
                PatientName = payment.Invoice.Patient!.FullName,
                Amount = payment.Amount,
                Method = payment.Method,
                PaidAt = payment.PaidAt,
                RemainingBalance = payment.Invoice.TotalDue - payment.Invoice.AmountPaid
            };
        }

        private static PaymentResponseDto MapToDto(Payment p, Invoice invoice) => new()
        {
            Id = p.Id,
            InvoiceId = p.InvoiceId,
            InvoiceNumber = invoice.InvoiceNumber,
            Amount = p.Amount,
            Method = p.Method,
            ReferenceNumber = p.ReferenceNumber,
            PaidAt = p.PaidAt,
            IsRefund = p.IsRefund,
            InvoiceStatus = invoice.Status,
            RemainingBalance = invoice.TotalDue - invoice.AmountPaid
        };
    }
}
