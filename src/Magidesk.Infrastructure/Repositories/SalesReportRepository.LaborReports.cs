using Magidesk.Application.DTOs.Reports;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

public partial class SalesReportRepository
{
    public async Task<ProductivityReportDto> GetServerProductivityAsync(DateTime startDate, DateTime endDate, Guid? userId, CancellationToken cancellationToken = default)
    {
        startDate = ToUtc(startDate);
        endDate = ToUtc(endDate);
        
        var report = new ProductivityReportDto
        {
            StartDate = ToSafeDisplayDate(startDate),
            EndDate = ToSafeDisplayDate(endDate)
        };

        // 1. Get Users (Servers)
        var usersQuery = _context.Users.AsNoTracking();
        if (userId.HasValue)
        {
            usersQuery = usersQuery.Where(u => u.Id == userId.Value);
        }
        var users = await usersQuery.ToListAsync(cancellationToken);

        // 2. Aggregate Data per User
        // We could do this in one big LINQ query, but splitting might be more readable and maintainable given the disparate sources (Tickets, Tips, Sessions).
        // Plus, we need to handle "Time Worked" via CashSessions which might be disjoint from Tickets.

        var serverStats = new List<ServerProductivityDto>();

        foreach (var user in users)
        {
            // A. Sales (Tickets Owned by User)
            var sales = await _context.Tickets
                .AsNoTracking()
                .Where(t => t.CreatedBy == new Domain.ValueObjects.UserId(user.Id) && t.ClosedAt >= startDate && t.ClosedAt <= endDate && t.Status == TicketStatus.Closed)
                .Select(t => t.SubtotalAmount.Amount) // Using Net Sales (Subtotal)
                .ToListAsync(cancellationToken);
            
            var totalSales = sales.Sum();
            var ticketCount = sales.Count;

            // B. Tips
            var tipsList = await _context.Gratuities
                .AsNoTracking()
                .Where(g => g.OwnerId.Value == user.Id && g.CreatedAt >= startDate && g.CreatedAt <= endDate)
                .Select(g => g.Amount.Amount)
                .ToListAsync(cancellationToken);
                
            var tips = tipsList.Sum();

            // C. Time Worked (CashSessions by User)
            var sessions = await _context.CashSessions
                .AsNoTracking()
                .Where(s => s.UserId == new Domain.ValueObjects.UserId(user.Id) && s.OpenedAt >= startDate && s.OpenedAt <= endDate)
                .Select(s => new { s.OpenedAt, s.ClosedAt })
                .ToListAsync(cancellationToken);

            double totalHours = 0;
            foreach (var s in sessions)
            {
                var endSession = s.ClosedAt ?? (DateTime.UtcNow > endDate ? endDate : DateTime.UtcNow);
                var duration = endSession - s.OpenedAt;
                totalHours += duration.TotalHours;
            }

            if (totalSales > 0 || totalHours > 0 || tips > 0)
            {
                serverStats.Add(new ServerProductivityDto
                {
                    UserId = user.Id,
                    UserName = $"{user.FirstName} {user.LastName}",
                    TotalSales = totalSales,
                    TipsCollected = tips,
                    TotalHours = Math.Round(totalHours, 2),
                    SalesPerHour = totalHours > 0 ? Math.Round(totalSales / (decimal)totalHours, 2) : 0,
                    TicketCount = ticketCount
                });
            }
        }

        report.ServerStats = serverStats.OrderByDescending(s => s.TotalSales).ToList();
        return report;
    }
    public async Task<LaborReportDto> GetLaborReportAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        startDate = ToUtc(startDate);
        endDate = ToUtc(endDate);
        
        // 1. Get All Active Users (with Roles if needed for RoleName)
        // We need to fetch users to get their names and HourlyRate.
        // Also fetch Role names if possible.
        // Assuming we can join Role.
        var users = await _context.Users
            .AsNoTracking()
            //.Include(u => u.Role) // If Role is navigation property.
            // RoleId is property. Role entity exists. Relation might not be configured as nav prop in UserConfiguration?
            // "Assuming relationship with Role is enforced at app layer..." commented out in Config.
            // So we might need to join Roles manualy or just fetch Roles separately.
            .Where(u => u.IsActive)
            .ToListAsync(cancellationToken);

        var roles = await _context.Roles.AsNoTracking().ToDictionaryAsync(r => r.Id, r => r.Name, cancellationToken);

        var report = new LaborReportDto
        {
            PeriodStart = ToSafeDisplayDate(startDate),
            PeriodEnd = ToSafeDisplayDate(endDate)
        };

        foreach (var user in users)
        {
            // Similar to Productivity Report
            
            // A. Time Worked (CashSessions)
            var sessions = await _context.CashSessions
                .AsNoTracking()
                .Where(s => s.UserId == new Domain.ValueObjects.UserId(user.Id) && s.OpenedAt >= startDate && s.OpenedAt <= endDate)
                .Select(s => new { s.OpenedAt, s.ClosedAt })
                .ToListAsync(cancellationToken);

            double totalHours = 0;
            foreach (var s in sessions)
            {
                var endSession = s.ClosedAt ?? (DateTime.UtcNow > endDate ? endDate : DateTime.UtcNow);
                var duration = endSession - s.OpenedAt;
                totalHours += duration.TotalHours;
            }

            // B. Sales (Net Sales) for Productivity KPI
            var sales = await _context.Tickets
                .AsNoTracking()
                .Where(t => t.CreatedBy == new Domain.ValueObjects.UserId(user.Id) && t.ClosedAt >= startDate && t.ClosedAt <= endDate && t.Status == TicketStatus.Closed)
                .Select(t => t.SubtotalAmount.Amount)
                .ToListAsync(cancellationToken);
            
            var totalSales = sales.Sum();

            // C. Calculate Labor Cost
            var hourlyRate = user.HourlyRate?.Amount ?? 0m;
            var laborCost = (decimal)totalHours * hourlyRate;

            if (totalHours > 0 || totalSales > 0)
            {
                var roleName = roles.ContainsKey(user.RoleId) ? roles[user.RoleId] : "Unknown";

                report.StaffLabor.Add(new LaborCostItemDto
                {
                    UserId = user.Id,
                    UserName = user.Username, // Or First/Last
                    RoleName = roleName,
                    TotalHours = totalHours,
                    HourlyRate = hourlyRate,
                    TotalCost = laborCost,
                    TotalSales = totalSales
                });
            }
        }

        // Aggregate Totals
        report.TotalLaborCost = report.StaffLabor.Sum(i => i.TotalCost);
        report.TotalNetSales = report.StaffLabor.Sum(i => i.TotalSales); // Determines Labor % relative to Attributed Sales?
        // Usually Labor % is relative to STORE Total Sales.
        // If we only sum attributed sales, we might miss sales from non-labor users?
        // But requested is "Hourly Labor Cost calculation".
        // Let's stick to Sum of item sales for now or should we fetch Global Sales?
        // F-0100 just says "Hourly Labor Cost".
        // Typically Labor Report compares Total Labor Cost vs Total Store Sales.
        // Let's fetch Total Store Sales separately to be accurate.
        
        var globalSales = await _context.Tickets
            .AsNoTracking()
            .Where(t => t.ClosedAt >= startDate && t.ClosedAt <= endDate && t.Status == TicketStatus.Closed)
            .SumAsync(t => t.SubtotalAmount.Amount, cancellationToken);
            
        report.TotalNetSales = globalSales;
        report.TotalHours = report.StaffLabor.Sum(i => i.TotalHours);

        return report;
    }
}
