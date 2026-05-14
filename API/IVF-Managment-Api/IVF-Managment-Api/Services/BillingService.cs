using IVF_Managment_Api.Data;
using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Models.HelperModels;
using IvfClinic.Models;
using Microsoft.EntityFrameworkCore;

namespace IVF_Managment_Api.Services;

public class BillingService : IBillingService
{
    private readonly IvfDbContext _db;
    private readonly IAuditLogger _audit;

    public BillingService(IvfDbContext db, IAuditLogger audit)
    {
        _db = db;
        _audit = audit;
    }

    public async Task<BillResponseDto> CreateBillAsync(CreateBillDto dto)
    {
        if (dto.Discount > 0 && string.IsNullOrWhiteSpace(dto.DiscountReason))
            throw new InvalidOperationException("DiscountReason is mandatory when a discount is applied.");

        var invoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpperInvariant()}";

        var entity = new Bill
        {
            Id = Guid.NewGuid(),
            PatientId = dto.PatientId,
            GeneratedByAdminId = dto.GeneratedByAdminId,
            InvoiceNumber = invoiceNumber,
            Subtotal = 0,
            TaxAmount = dto.Tax,
            InsuranceDeduction = dto.InsuranceDeduction,
            DiscountAmount = dto.Discount,
            DiscountReason = dto.DiscountReason,
            TotalDue = 0,
            Status = BillStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            LineItems = new List<BillLineItem>(),
            Payments = new List<Payment>()
        };

        _db.Bills.Add(entity);
        await _db.SaveChangesAsync();

        await _audit.LogAsync(dto.GeneratedByAdminId, "BillCreated", "Bill", entity.Id);

        return MapToResponse(entity);
    }

    public async Task<BillResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _db.Bills
            .AsNoTracking()
            .Include(b => b.LineItems)
            .FirstOrDefaultAsync(b => b.Id == id);

        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<IEnumerable<BillResponseDto>> GetByPatientAsync(Guid patientId)
    {
        var entities = await _db.Bills
            .AsNoTracking()
            .Include(b => b.LineItems)
            .Where(b => b.PatientId == patientId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return entities.Select(MapToResponse);
    }

    public async Task<BillLineItemResponseDto> AddLineItemAsync(CreateBillLineItemDto dto)
    {
        var bill = await _db.Bills.Include(b => b.LineItems).FirstOrDefaultAsync(b => b.Id == dto.BillId)
            ?? throw new InvalidOperationException("Bill not found.");

        var lineItem = new BillLineItem
        {
            Id = Guid.NewGuid(),
            BillId = dto.BillId,
            ServiceId = dto.ServiceId,
            Description = dto.Description,
            Amount = dto.Amount
        };

        _db.BillLineItems.Add(lineItem);

        RecomputeBillTotals(bill);

        await _db.SaveChangesAsync();

        return MapToLineItemResponse(lineItem);
    }

    public async Task<PaymentResponseDto> RecordPaymentAsync(CreatePaymentDto dto)
    {
        var bill = await _db.Bills
            .Include(b => b.Payments)
            .Include(b => b.LineItems)
            .FirstOrDefaultAsync(b => b.Id == dto.BillId)
            ?? throw new InvalidOperationException("Bill not found.");

        var totalPaid = bill.Payments.Sum(p => p.Amount);
        var outstanding = bill.TotalDue - totalPaid;

        if (dto.Amount > outstanding)
            throw new InvalidOperationException($"Payment of {dto.Amount} exceeds outstanding balance of {outstanding}.");

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            BillId = dto.BillId,
            RecordedByAdminId = dto.RecordedByUserId,
            Amount = dto.Amount,
            Method = dto.Method,
            PaymentDate = DateTime.UtcNow
        };

        _db.Payments.Add(payment);

        var newTotalPaid = totalPaid + dto.Amount;
        bill.Status = newTotalPaid >= bill.TotalDue ? BillStatus.Paid : BillStatus.PartiallyPaid;

        await _db.SaveChangesAsync();

        await _audit.LogAsync(dto.RecordedByUserId, "PaymentRecorded", "Payment", payment.Id);

        return new PaymentResponseDto
        {
            Id = payment.Id,
            BillId = payment.BillId,
            Amount = payment.Amount,
            Method = payment.Method,
            RecordedByUserId = payment.RecordedByAdminId,
            PaymentDate = payment.PaymentDate
        };
    }

    private static void RecomputeBillTotals(Bill bill)
    {
        bill.Subtotal = bill.LineItems.Sum(li => li.Amount);
        bill.TotalDue = bill.Subtotal + bill.TaxAmount - bill.InsuranceDeduction - bill.DiscountAmount;
        if (bill.TotalDue < 0) bill.TotalDue = 0;
    }

    private static BillResponseDto MapToResponse(Bill e) => new()
    {
        Id = e.Id,
        PatientId = e.PatientId,
        InvoiceNumber = e.InvoiceNumber,
        Subtotal = e.Subtotal,
        Tax = e.TaxAmount,
        InsuranceDeduction = e.InsuranceDeduction,
        Discount = e.DiscountAmount,
        DiscountReason = e.DiscountReason,
        TotalDue = e.TotalDue,
        Status = e.Status,
        CreatedAt = e.CreatedAt,
        LineItems = e.LineItems?.Select(MapToLineItemResponse).ToList() ?? new()
    };

    private static BillLineItemResponseDto MapToLineItemResponse(BillLineItem e) => new()
    {
        Id = e.Id,
        ServiceId = e.ServiceId,
        Description = e.Description,
        Amount = e.Amount
    };
}