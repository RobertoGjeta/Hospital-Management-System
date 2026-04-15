using Microsoft.EntityFrameworkCore;
using IVFClinic.Data;
using IVFClinic.DTOs.Billing;
using IVFClinic.Models;
using IVFClinic.Models.Enums;
using IVFClinic.Services.Interfaces;

namespace IVFClinic.Services.Implementations
{
    public class BillingService : IBillingService
    {
        private readonly AppDbContext _db;
   
        private readonly INotificationService _notif;
        private const decimal TaxRate = 0.20m; // 20% VAT — adjust per region

        public BillingService(AppDbContext db, IAuditService audit, INotificationService notif)
        {
            _db = db;
            _audit = audit;
            _notif = notif;
        }

        public async Task<IEnumerable<UnbilledServiceDto>> GetUnbilledServicesAsync(Guid patientId)
        {
            // Services delivered but not yet billed
            return await _db.RenderedServices
                .Include(rs => rs.Service)
                .Where(rs => rs.PatientId == patientId && !rs.IsBilled)
                .Select(rs => new UnbilledServiceDto
                {
                    RenderedServiceId = rs.Id,
                    ServiceId = rs.ServiceId,
                    ServiceName = rs.Service!.Name,
                    Category = rs.Service.Category,
                    DateRendered = rs.RenderedAt,
                    UnitPrice = rs.Service.Price,
                    Quantity = rs.Quantity,
                    LineTotal = rs.Service.Price * rs.Quantity
                })
                .ToListAsync();
        }

        public async Task<BillPreviewDto> PreviewBillAsync(Guid patientId, IEnumerable<Guid> serviceIds)
        {
            var ids = serviceIds.ToList();
            var items = await _db.RenderedServices
                .Include(rs => rs.Service)
                .Where(rs => rs.PatientId == patientId && ids.Contains(rs.Id) && !rs.IsBilled)
                .ToListAsync();

            var subtotal = items.Sum(i => i.Service!.Price * i.Quantity);
            var tax = subtotal * TaxRate;

            var patient = await _db.Patients.FindAsync(patientId);
            var hasInsurance = !string.IsNullOrEmpty(patient?.InsuranceProvider);
            var insuranceCoverage = hasInsurance ? subtotal * 0.70m : 0; // example: 70% coverage
            var patientPays = subtotal + tax - insuranceCoverage;

            return new BillPreviewDto
            {
                PatientId = patientId,
                Items = items.Select(i => new BillItemDto
                {
                    ServiceName = i.Service!.Name,
                    UnitPrice = i.Service.Price,
                    Quantity = i.Quantity,
                    LineTotal = i.Service.Price * i.Quantity
                }).ToList(),
                Subtotal = subtotal,
                Tax = tax,
                InsuranceCoverage = insuranceCoverage,
                TotalDue = patientPays
            };
        }

        public async Task<InvoiceResponseDto> GenerateBillAsync(BillCreateDto dto, Guid adminId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var preview = await PreviewBillAsync(dto.PatientId, dto.RenderedServiceIds);

                var invoice = new Invoice
                {
                    Id = Guid.NewGuid(),
                    InvoiceNumber = await GenerateInvoiceNumberAsync(),
                    PatientId = dto.PatientId,
                    Subtotal = preview.Subtotal,
                    Tax = preview.Tax,
                    InsuranceCoverage = preview.InsuranceCoverage,
                    Discount = dto.Discount ?? 0,
                    DiscountReason = dto.DiscountReason,
                    TotalDue = preview.TotalDue - (dto.Discount ?? 0),
                    Status = BillStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    CreatedById = adminId
                };

                _db.Invoices.Add(invoice);

                // Add line items and mark services as billed
                var rendered = await _db.RenderedServices
                    .Include(rs => rs.Service)
                    .Where(rs => dto.RenderedServiceIds.Contains(rs.Id))
                    .ToListAsync();

                foreach (var rs in rendered)
                {
                    _db.InvoiceItems.Add(new InvoiceItem
                    {
                        InvoiceId = invoice.Id,
                        ServiceId = rs.ServiceId,
                        Description = rs.Service!.Name,
                        UnitPrice = rs.Service.Price,
                        Quantity = rs.Quantity,
                        LineTotal = rs.Service.Price * rs.Quantity
                    });
                    rs.IsBilled = true;
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

               
                await _notif.SendBillNotificationAsync(invoice.Id);

                return await GetBillByIdAsync(invoice.Id) ?? throw new InvalidOperationException();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<InvoiceResponseDto> ApplyDiscountAsync(
            Guid invoiceId, decimal discount, string reason, Guid adminId)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Discount reason is mandatory");

            var invoice = await _db.Invoices.FindAsync(invoiceId)
                ?? throw new KeyNotFoundException("Invoice not found");

            var previousTotal = invoice.TotalDue;
            invoice.Discount = discount;
            invoice.DiscountReason = reason;
            invoice.TotalDue -= discount;
            await _db.SaveChangesAsync();

            await _audit.LogActionAsync(adminId, AuditAction.Update, "Invoice",
                invoiceId.ToString(),
                previousValues: new { previousTotal },
                newValues: new { discount, reason, newTotal = invoice.TotalDue },
                description: "Applied discount");

            return await GetBillByIdAsync(invoiceId) ?? throw new InvalidOperationException();
        }

        public async Task<InvoiceResponseDto?> GetBillByIdAsync(Guid invoiceId)
        {
            var invoice = await _db.Invoices
                .Include(i => i.Items)
                .Include(i => i.Patient)
                .FirstOrDefaultAsync(i => i.Id == invoiceId);
            return invoice == null ? null : MapToDto(invoice);
        }

        public async Task<IEnumerable<InvoiceResponseDto>> GetPatientBillsAsync(Guid patientId, BillStatus? status = null)
        {
            var query = _db.Invoices
                .Include(i => i.Items)
                .Where(i => i.PatientId == patientId);

            if (status.HasValue) query = query.Where(i => i.Status == status.Value);

            var invoices = await query.OrderByDescending(i => i.CreatedAt).ToListAsync();
            return invoices.Select(MapToDto);
        }

        private async Task<string> GenerateInvoiceNumberAsync()
        {
            var year = DateTime.UtcNow.Year;
            var count = await _db.Invoices.CountAsync(i => i.CreatedAt.Year == year);
            return $"INV-{year}-{(count + 1):D6}";
        }

        private static InvoiceResponseDto MapToDto(Invoice i) => new()
        {
            Id = i.Id,
            InvoiceNumber = i.InvoiceNumber,
            PatientId = i.PatientId,
            Subtotal = i.Subtotal,
            Tax = i.Tax,
            InsuranceCoverage = i.InsuranceCoverage,
            Discount = i.Discount,
            DiscountReason = i.DiscountReason,
            TotalDue = i.TotalDue,
            AmountPaid = i.AmountPaid,
            Status = i.Status,
            CreatedAt = i.CreatedAt,
            Items = i.Items?.Select(item => new BillItemDto
            {
                ServiceName = item.Description,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                LineTotal = item.LineTotal
            }).ToList()
        };
    }
}
