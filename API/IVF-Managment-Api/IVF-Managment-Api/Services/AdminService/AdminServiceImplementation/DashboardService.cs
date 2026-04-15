using Microsoft.EntityFrameworkCore;
using IVFClinic.Data;
using IVFClinic.DTOs.Dashboard;
using IVFClinic.Models.Enums;
using IVFClinic.Services.Interfaces;

namespace IVFClinic.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _db;

        public DashboardService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(Guid adminId)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);
            var sevenDaysAgo = today.AddDays(-7);

            // Run all queries in parallel for performance (NFR_A04: < 2 seconds)
            var todaysAppointmentsTask = _db.Appointments.CountAsync(a =>
                a.ScheduledStart >= today
                && a.ScheduledStart < tomorrow
                && a.Status != AppointmentStatus.Cancelled);

            var checkedInTask = _db.Appointments.CountAsync(a =>
                a.ScheduledStart >= today
                && a.ScheduledStart < tomorrow
                && a.Status == AppointmentStatus.CheckedIn);

            var availableRoomsTask = _db.Rooms.CountAsync(r =>
                r.Status == RoomStatus.Available);

            var pendingBillsCountTask = _db.Invoices.CountAsync(i =>
                i.Status == BillStatus.Pending || i.Status == BillStatus.PartiallyPaid);

            var pendingBillsAmountTask = _db.Invoices
                .Where(i => i.Status == BillStatus.Pending || i.Status == BillStatus.PartiallyPaid)
                .SumAsync(i => i.TotalDue - i.AmountPaid);

            var recentRegistrationsTask = _db.Patients
                .Where(p => p.CreatedAt >= sevenDaysAgo)
                .OrderByDescending(p => p.CreatedAt)
                .Take(10)
                .Select(p => new RecentRegistrationDto
                {
                    PatientId = p.Id,
                    FullName = p.FirstName + " " + p.LastName,
                    RegisteredAt = p.CreatedAt
                })
                .ToListAsync();

            await Task.WhenAll(
                todaysAppointmentsTask, checkedInTask, availableRoomsTask,
                pendingBillsCountTask, pendingBillsAmountTask, recentRegistrationsTask);

            return new DashboardSummaryDto
            {
                TodaysAppointments = todaysAppointmentsTask.Result,
                CheckedInPatients = checkedInTask.Result,
                AvailableRooms = availableRoomsTask.Result,
                PendingBillsCount = pendingBillsCountTask.Result,
                PendingBillsAmount = pendingBillsAmountTask.Result,
                RecentRegistrations = recentRegistrationsTask.Result,
                LoadedAt = DateTime.UtcNow
            };
        }
    }
}
