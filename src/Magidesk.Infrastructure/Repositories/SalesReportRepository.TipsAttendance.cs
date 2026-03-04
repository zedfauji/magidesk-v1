using Magidesk.Application.DTOs.Reports;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

public partial class SalesReportRepository
{
    public async Task<TipReportDto> GetTipReportAsync(DateTime startDate, DateTime endDate, Guid? userIdFilter = null, CancellationToken cancellationToken = default)
    {
        startDate = ToUtc(startDate);
        endDate = ToUtc(endDate);
        var report = new TipReportDto
        {
            StartDate = ToSafeDisplayDate(startDate),
            EndDate = ToSafeDisplayDate(endDate)
        };

        // 1. Fetch relevant tickets with their gratuities and payments
        var ticketsQuery = _context.Tickets
            .AsNoTracking()
            .Where(t => t.ClosedAt >= startDate && t.ClosedAt <= endDate && t.Status == TicketStatus.Closed)
            .Include(t => t.Gratuity)
            .Include(t => t.Payments)
            .AsQueryable();

        if (userIdFilter.HasValue)
        {
            ticketsQuery = ticketsQuery.Where(t => t.CreatedBy == new UserId(userIdFilter.Value));
        }

        var ticketsData = await ticketsQuery
            .Select(t => new
            {
                t.Id,
                t.TicketNumber,
                ServerId = (Guid)t.CreatedBy,
                TotalAmount = t.TotalAmount.Amount,
                Gratuity = t.Gratuity != null ? new { t.Gratuity.Amount.Amount, t.Gratuity.Paid } : null,
                Payments = t.Payments.Select(p => new { p.PaymentType, TipsAmount = p.TipsAmount.Amount })
            })
            .ToListAsync(cancellationToken);

        // 2. Resolve user names
        var serverIds = ticketsData.Select(t => t.ServerId).Distinct().ToList();
        var users = await _context.Users
            .AsNoTracking()
            .Where(u => serverIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.FirstName + " " + u.LastName, cancellationToken);

        foreach (var ticket in ticketsData)
        {
            var cashTips = ticket.Payments.Where(p => p.PaymentType == PaymentType.Cash).Sum(p => p.TipsAmount);
            var chargedTips = ticket.Payments.Where(p => p.PaymentType != PaymentType.Cash).Sum(p => p.TipsAmount);
            var autoGratuity = ticket.Gratuity?.Amount ?? 0;
            var totalTips = cashTips + chargedTips + autoGratuity;

            if (totalTips > 0)
            {
                var detail = new TipReportDataDto
                {
                    TicketId = ticket.TicketNumber.ToString(),
                    ServerName = users.GetValueOrDefault(ticket.ServerId, "Unknown"),
                    TicketTotal = ticket.TotalAmount,
                    CashTips = cashTips,
                    ChargedTips = chargedTips,
                    AutoGratuity = autoGratuity,
                    Tips = totalTips,
                    SaleType = chargedTips > 0 ? "Charged" : "Cash",
                    IsPaid = ticket.Gratuity?.Paid ?? true
                };

                report.Details.Add(detail);

                if (cashTips > 0 || autoGratuity > 0)
                {
                    report.CashTipsCount++;
                    report.CashTipsAmount += (cashTips + autoGratuity);
                }
                
                if (chargedTips > 0)
                {
                    report.ChargedTipsCount++;
                    report.ChargedTipsAmount += chargedTips;
                }

                report.TotalAutoGratuity += autoGratuity;

                if (detail.IsPaid)
                {
                    report.PaidTips += totalTips;
                }
                else
                {
                    report.TipsDue += totalTips;
                }
            }
        }

        report.TotalTips = report.CashTipsAmount + report.ChargedTipsAmount;
        
        // 3. Populate Server Summaries
        report.ServerSummaries = report.Details
            .GroupBy(d => d.ServerName)
            .Select(g => new TipReportServerSummaryDto
            {
                ServerName = g.Key,
                TicketCount = g.Count(),
                TotalSales = g.Sum(d => d.TicketTotal),
                CashTips = g.Sum(d => d.CashTips),
                ChargedTips = g.Sum(d => d.ChargedTips),
                AutoGratuity = g.Sum(d => d.AutoGratuity),
                TotalTips = g.Sum(d => d.Tips)
            })
            .ToList();

        if (report.Details.Count > 0)
        {
            report.AverageTips = report.TotalTips / report.Details.Count;
        }

        return report;
    }
    public async Task<AttendanceReportDto> GetAttendanceReportAsync(DateTime startDate, DateTime endDate, Guid? userIdFilter = null, CancellationToken cancellationToken = default)
    {
        startDate = ToUtc(startDate);
        endDate = ToUtc(endDate);

        var report = new AttendanceReportDto
        {
            StartDate = ToSafeDisplayDate(startDate),
            EndDate = ToSafeDisplayDate(endDate)
        };

        var query = _context.AttendanceHistories
            .AsNoTracking()
            .Where(a => a.ClockInTime >= startDate && a.ClockInTime <= endDate);

        if (userIdFilter.HasValue)
        {
            var userId = new UserId(userIdFilter.Value);
            query = query.Where(a => a.UserId == userId);
        }

        var events = await query
            .Select(a => new
            {
                a.Id,
                a.ClockInTime,
                a.ClockOutTime,
                a.UserId,
                a.ShiftId
            })
            .ToListAsync(cancellationToken);

        if (events.Count == 0)
        {
            return report;
        }

        var userIds = events.Select(e => e.UserId.Value).Distinct().ToList();
        var shiftsIds = events.Where(e => e.ShiftId.HasValue).Select(e => e.ShiftId!.Value).Distinct().ToList();

        var users = await _context.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new 
            { 
               u.Id, 
               Name = u.FirstName + " " + u.LastName,
               RoleName = u.Role != null ? u.Role.Name : "" 
            })
            .ToDictionaryAsync(u => u.Id, cancellationToken);
            
        var shifts = await _context.Shifts
            .AsNoTracking()
            .Where(s => shiftsIds.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, s => s.Name, cancellationToken);

        foreach (var evt in events)
        {
            var clockIn = ToSafeDisplayDate(evt.ClockInTime);
            DateTime? clockOut = null;
            if (evt.ClockOutTime.HasValue)
            {
                clockOut = ToSafeDisplayDate(evt.ClockOutTime.Value);
            }
            
            double duration = 0;
            if (clockOut.HasValue)
            {
                duration = (clockOut.Value - clockIn).TotalHours;
            }

            var userName = "Unknown";
            var roleName = "";
            if (users.TryGetValue(evt.UserId.Value, out var userData))
            {
                userName = userData.Name;
                roleName = userData.RoleName;
            }
            
            var shiftName = "";
            if (evt.ShiftId.HasValue && shifts.TryGetValue(evt.ShiftId.Value, out var sName))
            {
                shiftName = sName;
            }

            report.Items.Add(new AttendanceReportItemDto
            {
                UserId = evt.UserId.Value,
                UserName = userName,
                Role = roleName,
                ClockInTime = clockIn,
                ClockOutTime = clockOut,
                HoursWorked = duration,
                ShiftName = shiftName
            });
        }
        
        report.TotalHours = report.Items.Sum(i => i.HoursWorked);
        report.TotalShifts = report.Items.Count;

        return report;
    }
}
