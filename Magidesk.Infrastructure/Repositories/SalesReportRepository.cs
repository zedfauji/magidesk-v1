using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Enumerations;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

public class SalesReportRepository : ISalesReportRepository
{
    private readonly ApplicationDbContext _context;

    public SalesReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SalesBalanceReportDto> GetSalesBalanceAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        // 1. Fetch Closed Tickets in Range
        // We project to a lightweight anonymous type or fetch required fields to minimize data transfer if needed.
        // For now, fetching entities is acceptable if volume is not massive.
        
        var tickets = await _context.Tickets
            .AsNoTracking()
            .Where(t => t.ClosedAt >= startDate && t.ClosedAt <= endDate && t.Status == TicketStatus.Closed)
            .Select(t => new 
            {
                t.Id,
                TotalAmount = t.TotalAmount.Amount,
                SubtotalAmount = t.SubtotalAmount.Amount,
                TaxAmount = t.TaxAmount.Amount,
                DiscountAmount = t.DiscountAmount.Amount,
                ServiceChargeAmount = t.ServiceChargeAmount.Amount,
                DeliveryChargeAmount = t.DeliveryChargeAmount.Amount,
                GratuityAmount = t.Gratuity != null ? t.Gratuity.Amount.Amount : 0m
            })
            .ToListAsync(cancellationToken);

        // 2. Fetch Payments in Range
        var payments = await _context.Payments
            .AsNoTracking()
            .Where(p => p.TransactionTime >= startDate && p.TransactionTime <= endDate)
            .Where(p => !p.IsVoided)
            .Select(p => new
            {
                p.Amount.Amount,
                p.TransactionType,
                p.PaymentType
            })
            .ToListAsync(cancellationToken);

        // 3. Build Sales Summary
        var salesSummary = new SalesSummaryDto
        {
            TicketCount = tickets.Count,
            TotalGrossSales = tickets.Sum(t => t.TotalAmount),
            NetSales = tickets.Sum(t => t.SubtotalAmount),
            TaxAmount = tickets.Sum(t => t.TaxAmount),
            DiscountAmount = tickets.Sum(t => t.DiscountAmount),
            ServiceChargeAmount = tickets.Sum(t => t.ServiceChargeAmount),
            DeliveryChargeAmount = tickets.Sum(t => t.DeliveryChargeAmount),
            GratuityAmount = tickets.Sum(t => t.GratuityAmount)
        };
        
        // 4. Build Payment Summary
        var paymentSummary = new PaymentSummaryDto
        {
             TotalCollected = payments.Where(p => p.TransactionType == TransactionType.Credit).Sum(p => p.Amount),
             TotalRefunded = payments.Where(p => p.TransactionType == TransactionType.Debit).Sum(p => p.Amount)
        };
        
        // Group by type
        var paymentsByType = payments
            .GroupBy(p => p.PaymentType)
            .Select(g => new PaymentTypeSummaryDto
            {
                PaymentType = g.Key.ToString(),
                Count = g.Count(),
                // Net Amount for type = Credits - Debits
                Amount = g.Where(p => p.TransactionType == TransactionType.Credit).Sum(p => p.Amount) 
                         - g.Where(p => p.TransactionType == TransactionType.Debit).Sum(p => p.Amount)
            })
            .ToList();
            
        paymentSummary.ByType = paymentsByType;

        return new SalesBalanceReportDto
        {
            StartDate = startDate,
            EndDate = endDate,
            Sales = salesSummary,
            Payments = paymentSummary
        };
    }

    public async Task<SalesSummaryReportDto> GetSalesSummaryAsync(DateTime startDate, DateTime endDate, bool includeGroups, CancellationToken cancellationToken = default)
    {
        var tickets = await _context.Tickets
            .AsNoTracking()
            .Where(t => t.ClosedAt >= startDate && t.ClosedAt <= endDate && t.Status == TicketStatus.Closed)
            .Include(t => t.OrderLines)
            .ToListAsync(cancellationToken);

        var report = new SalesSummaryReportDto
        {
            StartDate = startDate,
            EndDate = endDate
        };

        var allLines = tickets.SelectMany(t => t.OrderLines).ToList();

        if (!allLines.Any()) return report;

        // Calculate Totals - Using SubtotalAmount as Gross based on business logic (Net = Total - Tax)
        report.Totals.TotalItemCount = allLines.Sum(l => l.ItemCount);
        report.Totals.TotalGrossSales = allLines.Sum(l => l.SubtotalAmount.Amount); 
        report.Totals.TotalNetSales = allLines.Sum(l => l.TotalAmount.Amount - l.TaxAmount.Amount);
        report.Totals.TotalTax = allLines.Sum(l => l.TaxAmount.Amount);

        // Group by Category
        var categoryGroups = allLines.GroupBy(l => l.CategoryName ?? "Uncategorized");

        foreach (var catGroup in categoryGroups)
        {
            var catDto = new SalesCategoryDto
            {
                Name = catGroup.Key,
                MainItemCount = catGroup.Sum(l => l.ItemCount),
                GrossSales = catGroup.Sum(l => l.SubtotalAmount.Amount),
                NetSales = catGroup.Sum(l => l.TotalAmount.Amount - l.TaxAmount.Amount),
                TaxAmount = catGroup.Sum(l => l.TaxAmount.Amount)
            };

            if (includeGroups)
            {
                var subGroups = catGroup.GroupBy(l => l.GroupName ?? "No Group");
                foreach (var subGroup in subGroups)
                {
                    catDto.Groups.Add(new SalesGroupDto
                    {
                        Name = subGroup.Key,
                        ItemCount = subGroup.Sum(l => l.ItemCount),
                        GrossSales = subGroup.Sum(l => l.SubtotalAmount.Amount)
                    });
                }
            }

            report.Categories.Add(catDto);
        }

        return report;
    }

    public async Task<ExceptionsReportDto> GetExceptionsReportAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var report = new ExceptionsReportDto
        {
            StartDate = startDate,
            EndDate = endDate
        };

        // 1. Voids
        // Fetch using projection to avoid full entity tracking
        var voids = await _context.Tickets
            .AsNoTracking()
            .Where(t => t.Status == TicketStatus.Voided && t.ActiveDate >= startDate && t.ActiveDate <= endDate)
            .Select(t => new VoidItemDto
            {
                Date = t.ActiveDate,
                TicketNumber = t.TicketNumber,
                Amount = t.TotalAmount.Amount,
                VoidedBy = t.VoidedBy != null ? t.VoidedBy.Value.ToString() : "Unknown", // Handle nullable UserId
                Reason = "Voided" // Placeholder
            })
            .ToListAsync(cancellationToken);
        
        report.Voids = voids;

        // 2. Refunds
        var refunds = await _context.Payments
            .AsNoTracking()
            .Where(p => p.TransactionType == TransactionType.Debit && p.TransactionTime >= startDate && p.TransactionTime <= endDate)
            .Join(_context.Tickets, 
                  p => p.TicketId, 
                  t => t.Id, 
                  (p, t) => new { p, t.TicketNumber })
            .Select(x => new RefundItemDto
            {
                Date = x.p.TransactionTime,
                TicketNumber = x.TicketNumber,
                Amount = x.p.Amount.Amount,
                PaymentType = x.p.PaymentType.ToString(),
                Reason = x.p.Note ?? "Refund"
            })
            .ToListAsync(cancellationToken);

        report.Refunds = refunds;

        // 3. Discounts
        var discounts = await _context.TicketDiscounts
            .AsNoTracking()
            .Where(d => d.AppliedAt >= startDate && d.AppliedAt <= endDate)
            .Join(_context.Tickets,
                  d => d.TicketId,
                  t => t.Id,
                  (d, t) => new { d, t.TicketNumber })
            .Select(x => new DiscountItemDto
            {
                Date = x.d.AppliedAt,
                TicketNumber = x.TicketNumber,
                Name = x.d.Name,
                Amount = x.d.Amount.Amount
            })
            .ToListAsync(cancellationToken);

        report.Discounts = discounts;

        return report;
    }

    public async Task<JournalReportDto> GetJournalReportAsync(DateTime startDate, DateTime endDate, string? entityType, Guid? userId, CancellationToken cancellationToken = default)
    {
        var query = _context.AuditEvents
            .AsNoTracking()
            .Where(e => e.Timestamp >= startDate && e.Timestamp <= endDate);

        if (!string.IsNullOrWhiteSpace(entityType))
        {
            query = query.Where(e => e.EntityType == entityType);
        }

        if (userId.HasValue)
        {
            query = query.Where(e => e.UserId == userId.Value);
        }

        var entries = await query
            .Join(_context.Users,
                  e => e.UserId,
                  u => u.Id,
                  (e, u) => new { Event = e, UserName = u.FirstName + " " + u.LastName })
            .OrderByDescending(x => x.Event.Timestamp)
            .Select(x => new JournalEntryDto
            {
                Timestamp = x.Event.Timestamp,
                EventType = x.Event.EventType.ToString(),
                Description = x.Event.Description,
                User = x.UserName,
                BeforeState = x.Event.BeforeState ?? string.Empty,
                AfterState = x.Event.AfterState
            })
            .ToListAsync(cancellationToken);

        return new JournalReportDto
        {
            StartDate = startDate,
            EndDate = endDate,
            Entries = entries
        };
    }

    public async Task<ProductivityReportDto> GetServerProductivityAsync(DateTime startDate, DateTime endDate, Guid? userId, CancellationToken cancellationToken = default)
    {
        var report = new ProductivityReportDto
        {
            StartDate = startDate,
            EndDate = endDate
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
                    TotalTips = tips,
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
            PeriodStart = startDate,
            PeriodEnd = endDate
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
    public async Task<DeliveryReportDto> GetDeliveryReportAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        // 1. Fetch Closed or Paid Tickets in Range that are Type "Delivery" (or just by AssginedDriverId?)
        // Better: Fetch ticket where AssignedDriverId is not null or Order Type is Delivery.
        // Let's filter by range first.
        
        var query = _context.Tickets
            .AsNoTracking()
            .Where(t => t.ClosedAt >= startDate && t.ClosedAt <= endDate && t.Status == TicketStatus.Closed && t.AssignedDriverId != null);
            // We use AssignedDriverId != null as primary signal for "Delivery that involves a driver".
            // If we filter by OrderType Name containing 'Delivery', we skip orders that might have been delivered but type wasn't exact match?
            // "AssignedDriverId" is the strongest signal for "Driver Performance".
            
        var tickets = await query
            .Select(t => new 
            {
                t.Id,
                t.AssignedDriverId,
                Subtotal = t.SubtotalAmount.Amount,
                Tax = t.TaxAmount.Amount,
                Total = t.TotalAmount.Amount,
                t.ClosedAt,
                t.DispatchedTime, // Needed for Time Calculation
                t.DeliveryAddress,
                t.Properties // Check for gratuity? Or use Gratuity entity?
                // Gratuity is navigation property.
            })
            .ToListAsync(cancellationToken);

        // Fetch Driver Names
        var driverIds = tickets.Select(t => t.AssignedDriverId).Distinct().Where(id => id.HasValue).Select(id => id!.Value).ToList();
        var drivers = await _context.Users
            .AsNoTracking()
            .Where(u => driverIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Username, cancellationToken); // Or FirstName

        // Fetch Tips Separately (if gratuity is not easily selected in anonymous projection without Include)
        // Or we can load tips by TicketIds.
        var ticketIds = tickets.Select(t => t.Id).ToList();
        var tips = await _context.Gratuities
            .AsNoTracking()
            .Where(g => ticketIds.Contains(g.TicketId))
            .Select(g => new { g.TicketId, Amount = g.Amount.Amount })
            .ToDictionaryAsync(g => g.TicketId, g => g.Amount, cancellationToken);

        var report = new DeliveryReportDto
        {
            PeriodStart = startDate,
            PeriodEnd = endDate
        };

        // Aggregation
        var driverStats = tickets
            .GroupBy(t => t.AssignedDriverId)
            .Select(g => 
            {
                var driverId = g.Key!.Value;
                var driverName = drivers.ContainsKey(driverId) ? drivers[driverId] : "Unknown";
                
                var count = g.Count();
                var totalSales = g.Sum(x => x.Total); // Use TotalAmount (Gross) for settlement/cashiering, or Subtotal for performance?
                // Usually Driver "Sales" refers to money handled or value delivered. Let's use TotalAmount.
                
                // Tips
                var driverTips = g.Sum(x => tips.ContainsKey(x.Id) ? tips[x.Id] : 0m);

                // Time (Close - Dispatch)
                // DispatchedTime might be null if not tracked properly? Default to OpenedAt? 
                // Let's protect against null DispatchedTime.
                var timeSum = g.Sum(x => 
                {
                   if (x.DispatchedTime.HasValue && x.ClosedAt.HasValue)
                   {
                       return (x.ClosedAt.Value - x.DispatchedTime.Value).TotalMinutes;
                   }
                   return 0;
                });
                
                var validTimeCount = g.Count(x => x.DispatchedTime.HasValue && x.ClosedAt.HasValue);
                var avgTime = validTimeCount > 0 ? timeSum / validTimeCount : 0;

                return new DriverPerformanceDto
                {
                    DriverId = driverId,
                    DriverName = driverName,
                    DeliveryCount = count,
                    TotalSales = totalSales,
                    TipsAmount = driverTips,
                    AverageTimeMinutes = Math.Round(avgTime, 1)
                };
            })
            .OrderByDescending(d => d.DeliveryCount)
            .ToList();

        report.DriverStats = driverStats;
        report.TotalDeliveries = driverStats.Sum(d => d.DeliveryCount);
        report.TotalDeliverySales = driverStats.Sum(d => d.TotalSales);
        
        if (report.TotalDeliveries > 0)
        {
             // Weighted average for overall time? Or simple average of averages?
             // Simple average of averages is incorrect.
             // Re-calculate global average.
             var globalTimeSum = tickets
                 .Where(x => x.DispatchedTime.HasValue && x.ClosedAt.HasValue)
                 .Sum(x => (x.ClosedAt!.Value - x.DispatchedTime!.Value).TotalMinutes);
                 
             var globalValidCount = tickets.Count(x => x.DispatchedTime.HasValue && x.ClosedAt.HasValue);
             report.AverageDeliveryTimeMinutes = globalValidCount > 0 ? Math.Round(globalTimeSum / globalValidCount, 1) : 0;
        }

        return report;
    }
}
