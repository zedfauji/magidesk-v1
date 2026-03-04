using Magidesk.Application.DTOs.Reports;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

public partial class SalesReportRepository
{
    public async Task<ServerProductivityReportDto> GetServerProductivityReportAsync(DateTime startDate, DateTime endDate, Guid? userIdFilter = null, CancellationToken cancellationToken = default)
    {
        startDate = ToUtc(startDate);
        endDate = ToUtc(endDate);
        var report = new ServerProductivityReportDto
        {
            StartDate = ToSafeDisplayDate(startDate),
            EndDate = ToSafeDisplayDate(endDate)
        };

        // Build base query for tickets in date range
        var query = _context.Tickets
            .AsNoTracking()
            .Where(t => t.ClosedAt >= startDate && t.ClosedAt <= endDate && t.Status == TicketStatus.Closed);

        // Apply user filter if provided
        if (userIdFilter.HasValue)
        {
            query = query.Where(t => t.CreatedBy == new UserId(userIdFilter.Value));
        }

        // Execute query and project to anonymous type first to handle user mapping
        var data = await query
            .Select(t => new
            {
                ServerId = (Guid)t.CreatedBy,
                TicketCount = 1,
                TotalAmount = t.TotalAmount.Amount,
                PaidAmount = t.PaidAmount.Amount,
                TipsAmount = t.Payments.Sum(p => p.TipsAmount.Amount) + (t.Gratuity != null ? t.Gratuity.Amount.Amount : 0m),
                t.CreatedAt,
                t.ClosedAt
            })
            .ToListAsync(cancellationToken);

        // Resolve user names
        var serverIds = data.Select(i => i.ServerId).Distinct().ToList();
        var users = await _context.Users
            .AsNoTracking()
            .Where(u => serverIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => $"{u.FirstName} {u.LastName}", cancellationToken);

        // Group by server
        var groupedServers = data
            .GroupBy(t => t.ServerId)
            .Select(g => new ServerProductivityDto
            {
                UserId = g.Key,
                UserName = users.GetValueOrDefault(g.Key, "Unknown"),
                TicketCount = g.Count(),
                TotalSales = g.Sum(t => t.TotalAmount),
                NetSales = g.Sum(t => t.TotalAmount - t.TipsAmount),
                TipsCollected = g.Sum(t => t.TipsAmount),
                AverageTicketSize = g.Count() > 0 ? g.Sum(t => t.TotalAmount) / g.Count() : 0,
                AverageTipPercentage = g.Sum(t => t.TotalAmount) > 0 ? (g.Sum(t => t.TipsAmount) / g.Sum(t => t.TotalAmount)) * 100 : 0,
                TotalHours = CalculateWorkHours(g.Select(x => new { x.CreatedAt, ClosedAt = x.ClosedAt ?? x.CreatedAt }).Cast<dynamic>().ToList()),
                SalesPerHour = CalculateWorkHours(g.Select(x => new { x.CreatedAt, ClosedAt = x.ClosedAt ?? x.CreatedAt }).Cast<dynamic>().ToList()) > 0 
                               ? g.Sum(t => t.TotalAmount) / (decimal)CalculateWorkHours(g.Select(x => new { x.CreatedAt, ClosedAt = x.ClosedAt ?? x.CreatedAt }).Cast<dynamic>().ToList()) : 0
            })
            .OrderByDescending(g => g.TotalSales)
            .ToList();

        report.Servers = groupedServers;

        // Calculate totals
        report.Totals.TotalServers = groupedServers.Count;
        report.Totals.TotalTickets = groupedServers.Sum(g => g.TicketCount);
        report.Totals.TotalSales = groupedServers.Sum(g => g.TotalSales);
        report.Totals.TotalNetSales = groupedServers.Sum(g => g.NetSales);
        report.Totals.TotalTips = groupedServers.Sum(g => g.TipsCollected);
        report.Totals.AverageTicketSize = report.Totals.TotalTickets > 0 ? report.Totals.TotalSales / report.Totals.TotalTickets : 0;
        report.Totals.AverageTipPercentage = report.Totals.TotalSales > 0 ? (report.Totals.TotalTips / report.Totals.TotalSales) * 100 : 0;
        var totalWorkHours = groupedServers.Sum(g => g.TotalHours);
        report.Totals.AverageSalesPerHour = totalWorkHours > 0 ? report.Totals.TotalSales / (decimal)totalWorkHours : 0;

        return report;
    }
    private static double CalculateWorkHours(List<dynamic> tickets)
    {
        if (!tickets.Any()) return 0;

        var firstTicket = tickets.OrderBy(t => t.CreatedAt).First();
        var lastTicket = tickets.OrderBy(t => t.ClosedAt).Last();
        
        // Simple calculation: time between first ticket created and last ticket closed
        // In a real implementation, this would consider actual shift schedules
        var timeSpan = lastTicket.ClosedAt - firstTicket.CreatedAt;
        return Math.Max(1, timeSpan.TotalHours); // Minimum 1 hour to avoid division by zero
    }
    public async Task<HourlyLaborReportDto> GetHourlyLaborReportAsync(DateTime startDate, DateTime endDate, Guid? employeeIdFilter = null, CancellationToken cancellationToken = default)
    {
        startDate = ToUtc(startDate);
        endDate = ToUtc(endDate);
        var report = new HourlyLaborReportDto
        {
            StartDate = ToSafeDisplayDate(startDate),
            EndDate = ToSafeDisplayDate(endDate)
        };

        // Build base query for attendance histories in date range
        var query = _context.AttendanceHistories
            .AsNoTracking()
            .Where(cio => cio.ClockInTime >= startDate && (cio.ClockOutTime == null || cio.ClockOutTime <= endDate));

        // Apply employee filter if provided
        if (employeeIdFilter.HasValue)
        {
            query = query.Where(cio => cio.UserId == new UserId(employeeIdFilter.Value));
        }

        // Execute query and group by hour
        var clockEntries = await query
            .Select(cio => new
            {
                UserId = (Guid)cio.UserId,
                Hour = cio.ClockInTime.Hour,
                cio.ClockInTime,
                ClockOutTime = cio.ClockOutTime ?? DateTime.UtcNow,
            })
            .ToListAsync(cancellationToken);

        // Resolve user names and wages
        var userIds = clockEntries.Select(e => e.UserId).Distinct().ToList();
        var users = await _context.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => new { Name = $"{u.FirstName} {u.LastName}", Wage = u.HourlyRate?.Amount ?? 0m }, cancellationToken);

        // Pre-compute hourly sales so we can look them up in the in-memory projection below
        // (GetSalesForHour cannot be async while inside a LINQ Select chain)
        var hourlySales = await _context.Tickets
            .AsNoTracking()
            .Where(t => t.ClosedAt >= startDate && t.ClosedAt <= endDate && t.Status == TicketStatus.Closed)
            .Select(t => new { Hour = t.ClosedAt!.Value.Hour, TotalAmount = t.TotalAmount.Amount })
            .ToListAsync(cancellationToken);

        var hourlySalesLookup = hourlySales
            .GroupBy(t => t.Hour)
            .ToDictionary(g => g.Key, g => g.Sum(t => t.TotalAmount));

        // Group by hour and employee
        var hourlyGroups = clockEntries
            .Select(cio => new
            {
                cio.Hour,
                cio.UserId,
                UserName = users.GetValueOrDefault(cio.UserId)?.Name ?? "Unknown",
                HoursWorked = (decimal)(cio.ClockOutTime - cio.ClockInTime).TotalHours,
                Wage = users.GetValueOrDefault(cio.UserId)?.Wage ?? 0m
            })
            .GroupBy(cio => new { cio.Hour, cio.UserId, cio.UserName })
            .Select(g => new
            {
                g.Key.Hour,
                g.Key.UserId,
                g.Key.UserName,
                HoursWorked = g.Sum(cio => cio.HoursWorked),
                LaborCost = g.Sum(cio => cio.HoursWorked * cio.Wage)
            })
            .GroupBy(x => x.Hour)
            .Select(h => new HourlyLaborDto
            {
                Hour = h.Key,
                Employees = h.Select(e => new EmployeeLaborDto
                {
                    EmployeeId = e.UserId,
                    EmployeeName = e.UserName,
                    HoursWorked = e.HoursWorked,
                    LaborCost = e.LaborCost
                }).ToList(),
                TotalLaborHours = h.Sum(e => e.HoursWorked),
                TotalLaborCost = h.Sum(e => e.LaborCost),
                TotalSales = hourlySalesLookup.GetValueOrDefault(h.Key, 0m),
                LaborPercentage = 0
            })
            .OrderBy(g => g.Hour)
            .ToList();

        // Calculate labor percentages
        var totalSales = hourlyGroups.Sum(g => g.TotalSales);
        foreach (var hour in hourlyGroups)
        {
            hour.LaborPercentage = hour.TotalSales > 0 ? (hour.TotalLaborCost / hour.TotalSales) * 100 : 0;
            hour.IsHighLaborPercentage = hour.LaborPercentage > 15.0m;
        }

        report.Hours = hourlyGroups;

        // Calculate totals
        report.Totals.TotalLaborHours = hourlyGroups.Sum(g => g.TotalLaborHours);
        report.Totals.TotalLaborCost = hourlyGroups.Sum(g => g.TotalLaborCost);
        report.Totals.TotalSales = totalSales;
        report.Totals.AverageLaborPercentage = totalSales > 0 ? (report.Totals.TotalLaborCost / totalSales) * 100 : 0;
        report.Totals.TotalEmployees = hourlyGroups.SelectMany(g => g.Employees).Select(e => e.EmployeeId).Distinct().Count();

        return report;
    }
}
