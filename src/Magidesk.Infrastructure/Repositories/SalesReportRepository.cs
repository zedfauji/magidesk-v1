using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Enumerations;
using Magidesk.Infrastructure.Data;
using Magidesk.Domain.ValueObjects;
using Magidesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

public class SalesReportRepository : ISalesReportRepository
{
    private readonly ApplicationDbContext _context;

    public SalesReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    private static DateTime ToUtc(DateTime dt)
    {
        if (dt.Kind == DateTimeKind.Utc) return dt;
        
        // Clamp to a range that is safe for all timezones and drivers (1900-9999)
        if (dt < new DateTime(1900, 1, 1)) return new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        if (dt > new DateTime(9999, 12, 30)) return new DateTime(9999, 12, 30, 23, 59, 59, DateTimeKind.Utc);
        
        try
        {
            return dt.ToUniversalTime();
        }
        catch (ArgumentOutOfRangeException)
        {
            // If conversion fails due to timezone shift, fallback to SpecifyKind
            return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
        }
    }

    private static DateTime ToSafeDisplayDate(DateTime dt)
    {
        // Many UI frameworks (like WinUI 3) crash or behave poorly with year 1 dates.
        // Clamping to a safe range for display.
        if (dt < new DateTime(1900, 1, 1)) return new DateTime(1900, 1, 1);
        if (dt > new DateTime(9999, 12, 30)) return new DateTime(9999, 12, 30);
        return dt;
    }

    public async Task<SalesBalanceReportDto> GetSalesBalanceAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        startDate = ToUtc(startDate);
        endDate = ToUtc(endDate);
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
            StartDate = ToSafeDisplayDate(startDate),
            EndDate = ToSafeDisplayDate(endDate),
            Sales = salesSummary,
            Payments = paymentSummary
        };
    }

    public async Task<SalesSummaryReportDto> GetSalesSummaryAsync(DateTime startDate, DateTime endDate, bool includeGroups, CancellationToken cancellationToken = default)
    {
        startDate = ToUtc(startDate);
        endDate = ToUtc(endDate);

        var report = new SalesSummaryReportDto
        {
            StartDate = ToSafeDisplayDate(startDate),
            EndDate = ToSafeDisplayDate(endDate)
        };

        // Use a projected join to avoid materializing problematic entity fields
        var allLines = await (from ol in _context.OrderLines.AsNoTracking()
                             join t in _context.Tickets.AsNoTracking() on ol.TicketId equals t.Id
                             where t.ClosedAt >= startDate && t.ClosedAt <= endDate && t.Status == TicketStatus.Closed
                             select new 
                             {
                                 ol.ItemCount,
                                 SubtotalAmount = ol.SubtotalAmount.Amount,
                                 TotalAmount = ol.TotalAmount.Amount,
                                 TaxAmount = ol.TaxAmount.Amount,
                                 CategoryName = ol.CategoryName ?? "Uncategorized",
                                 GroupName = ol.GroupName ?? "No Group"
                             })
                             .ToListAsync(cancellationToken);

        if (!allLines.Any()) return report;

        // Calculate Totals - Using SubtotalAmount as Gross based on business logic (Net = Total - Tax)
        report.Totals.TotalItemCount = allLines.Sum(l => l.ItemCount);
        report.Totals.TotalGrossSales = allLines.Sum(l => l.SubtotalAmount); 
        report.Totals.TotalNetSales = allLines.Sum(l => l.TotalAmount - l.TaxAmount);
        report.Totals.TotalTax = allLines.Sum(l => l.TaxAmount);

        // Group by Category
        var categoryGroups = allLines.GroupBy(l => l.CategoryName);

        foreach (var catGroup in categoryGroups)
        {
            var catDto = new SalesCategoryDto
            {
                Name = catGroup.Key,
                MainItemCount = catGroup.Sum(l => l.ItemCount),
                GrossSales = catGroup.Sum(l => l.SubtotalAmount),
                NetSales = catGroup.Sum(l => l.TotalAmount - l.TaxAmount),
                TaxAmount = catGroup.Sum(l => l.TaxAmount)
            };

            if (includeGroups)
            {
                var subGroups = catGroup.GroupBy(l => l.GroupName);
                foreach (var subGroup in subGroups)
                {
                    catDto.Groups.Add(new SalesGroupDto
                    {
                        Name = subGroup.Key,
                        ItemCount = subGroup.Sum(l => l.ItemCount),
                        GrossSales = subGroup.Sum(l => l.SubtotalAmount)
                    });
                }
            }

            report.Categories.Add(catDto);
        }

        return report;
    }

    public async Task<ExceptionsReportDto> GetExceptionsReportAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        startDate = ToUtc(startDate);
        endDate = ToUtc(endDate);
        var report = new ExceptionsReportDto
        {
            StartDate = ToSafeDisplayDate(startDate),
            EndDate = ToSafeDisplayDate(endDate)
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
        
        foreach(var v in voids) v.Date = ToSafeDisplayDate(v.Date);
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

        foreach(var r in refunds) r.Date = ToSafeDisplayDate(r.Date);
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

        foreach(var d in discounts) d.Date = ToSafeDisplayDate(d.Date);
        report.Discounts = discounts;

        return report;
    }

    public async Task<JournalReportDto> GetJournalReportAsync(DateTime startDate, DateTime endDate, string? entityType, Guid? userId, CancellationToken cancellationToken = default)
    {
        startDate = ToUtc(startDate);
        endDate = ToUtc(endDate);
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

        foreach (var entry in entries) entry.Timestamp = ToSafeDisplayDate(entry.Timestamp);

        return new JournalReportDto
        {
            StartDate = ToSafeDisplayDate(startDate),
            EndDate = ToSafeDisplayDate(endDate),
            Entries = entries
        };
    }

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
    public async Task<SalesDetailReportDto> GetSalesDetailAsync(DateTime startDate, DateTime endDate, string? categoryFilter = null, string? groupFilter = null, string? itemFilter = null, CancellationToken cancellationToken = default)
    {
        startDate = ToUtc(startDate);
        endDate = ToUtc(endDate);
        var report = new SalesDetailReportDto
        {
            StartDate = ToSafeDisplayDate(startDate),
            EndDate = ToSafeDisplayDate(endDate)
        };

        // Build base query for ticket items
        var ticketsQuery = _context.Tickets
            .AsNoTracking()
            .Where(t => t.ClosedAt >= startDate && t.ClosedAt <= endDate && t.Status == TicketStatus.Closed);

        var query = ticketsQuery.SelectMany(t => t.OrderLines.Select(l => new
        {
            Ticket = t,
            Line = l
        }));

        // Apply filters if provided
        if (!string.IsNullOrWhiteSpace(categoryFilter))
        {
            query = query.Where(x => x.Line.CategoryName == categoryFilter);
        }

        if (!string.IsNullOrWhiteSpace(groupFilter))
        {
            query = query.Where(x => x.Line.GroupName == groupFilter);
        }

        if (!string.IsNullOrWhiteSpace(itemFilter))
        {
            query = query.Where(x => x.Line.MenuItemName.Contains(itemFilter));
        }

        // Execute query and project to anonymous type first to handle user mapping
        var data = await query
            .Select(x => new
            {
                TicketTime = x.Ticket.ClosedAt ?? x.Ticket.CreatedAt,
                TicketNumber = x.Ticket.TicketNumber,
                ItemName = x.Line.MenuItemName,
                CategoryName = x.Line.CategoryName ?? "Uncategorized",
                GroupName = x.Line.GroupName ?? "No Group",
                Quantity = (int)x.Line.ItemCount,
                UnitPrice = x.Line.UnitPrice.Amount,
                GrossAmount = x.Line.SubtotalAmount.Amount,
                DiscountAmount = x.Line.DiscountAmount.Amount,
                NetAmount = x.Line.TotalAmount.Amount - x.Line.TaxAmount.Amount,
                TaxAmount = x.Line.TaxAmount.Amount,
                ServerId = (Guid)x.Ticket.CreatedBy
            })
            .ToListAsync(cancellationToken);

        // Resolve user names
        var serverIds = data.Select(i => i.ServerId).Distinct().ToList();
        var users = await _context.Users
            .AsNoTracking()
            .Where(u => serverIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => $"{u.FirstName} {u.LastName}", cancellationToken);

        report.Items = data.Select(i => new SalesDetailItemDto
        {
            TicketTime = ToSafeDisplayDate(i.TicketTime),
            TicketNumber = i.TicketNumber.ToString(),
            ItemName = i.ItemName,
            CategoryName = i.CategoryName,
            GroupName = i.GroupName,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            GrossAmount = i.GrossAmount,
            DiscountAmount = i.DiscountAmount,
            NetAmount = i.NetAmount,
            TaxAmount = i.TaxAmount,
            UserName = users.GetValueOrDefault(i.ServerId, "Unknown")
        }).ToList();

        // Calculate totals
        report.Totals.TotalItems = report.Items.Sum(i => i.Quantity);
        report.Totals.TotalGrossSales = report.Items.Sum(i => i.GrossAmount);
        report.Totals.TotalDiscounts = report.Items.Sum(i => i.DiscountAmount);
        report.Totals.TotalNetSales = report.Items.Sum(i => i.NetAmount);
        report.Totals.TotalTax = report.Items.Sum(i => i.TaxAmount);

        return report;
    }

    public async Task<CreditCardReportDto> GetCreditCardReportAsync(DateTime startDate, DateTime endDate, string? cardTypeFilter = null, string? transactionTypeFilter = null, CancellationToken cancellationToken = default)
    {
        startDate = ToUtc(startDate);
        endDate = ToUtc(endDate);
        var report = new CreditCardReportDto
        {
            StartDate = ToSafeDisplayDate(startDate),
            EndDate = ToSafeDisplayDate(endDate)
        };

        // Build base query for credit card payments
        var query = _context.Payments
            .OfType<CreditCardPayment>()
            .AsNoTracking()
            .Where(p => p.TransactionTime >= startDate && p.TransactionTime <= endDate)
            .Join(_context.Tickets,
                  p => p.TicketId,
                  t => t.Id,
                  (p, t) => new { p, t });

        // Apply filters if provided
        if (!string.IsNullOrWhiteSpace(cardTypeFilter))
        {
            query = query.Where(x => x.p.CardType != null && x.p.CardType.Contains(cardTypeFilter));
        }

        if (!string.IsNullOrWhiteSpace(transactionTypeFilter))
        {
            query = query.Where(x => x.p.TransactionType.ToString().Contains(transactionTypeFilter));
        }

        // Execute query and project to DTO
        var transactions = await query
            .Select(x => new CreditCardTransactionDto
            {
                TransactionTime = x.p.TransactionTime,
                TicketNumber = x.t.TicketNumber.ToString(),
                CardType = x.p.CardType ?? "Unknown",
                CardLast4 = x.p.CardNumber != null && x.p.CardNumber.Length >= 4 ? "**** " + x.p.CardNumber.Substring(x.p.CardNumber.Length - 4) : "****",
                AuthorizationCode = x.p.AuthorizationCode ?? string.Empty,
                Amount = x.p.Amount.Amount,
                TipAmount = x.p.TipsAmount.Amount,
                TransactionType = x.p.TransactionType.ToString(),
                TransactionStatus = x.p.IsVoided ? "Voided" : "Approved",
                TerminalId = x.p.TerminalId.ToString(),
                MerchantId = string.Empty
            })
            .OrderByDescending(x => x.TransactionTime)
            .ToListAsync(cancellationToken);

        report.Transactions = transactions;

        // Calculate totals
        report.Totals.TotalTransactions = transactions.Count;
        report.Totals.TotalSales = transactions.Where(t => t.TransactionType == "Credit").Sum(t => t.Amount);
        report.Totals.TotalTips = transactions.Sum(t => t.TipAmount);
        report.Totals.TotalVoids = transactions.Where(t => t.TransactionStatus == "Voided").Sum(t => t.Amount);
        report.Totals.TotalRefunds = transactions.Where(t => t.TransactionType == "Debit" && t.TransactionStatus != "Voided").Sum(t => t.Amount);

        // Group by card type
        var cardTypeGroups = transactions
            .GroupBy(t => t.CardType)
            .Select(g => new CardTypeTotalDto
            {
                CardType = g.Key,
                TransactionCount = g.Count(),
                TotalAmount = g.Sum(t => t.Amount),
                TipAmount = g.Sum(t => t.TipAmount)
            })
            .OrderByDescending(g => g.TotalAmount)
            .ToList();

        report.Totals.ByCardType = cardTypeGroups;

        return report;
    }

    private static string MaskCardNumber(string? cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length < 4)
            return "****";

        // Return only last 4 digits with masking for PCI compliance
        return "**** **** **** " + cardNumber.Substring(Math.Max(0, cardNumber.Length - 4));
    }

    public async Task<PaymentReportDto> GetPaymentReportAsync(DateTime startDate, DateTime endDate, string? terminalFilter = null, CancellationToken cancellationToken = default)
    {
        startDate = ToUtc(startDate);
        endDate = ToUtc(endDate);
        var report = new PaymentReportDto
        {
            StartDate = ToSafeDisplayDate(startDate),
            EndDate = ToSafeDisplayDate(endDate)
        };

        // Build base query for all payments in date range
        var query = _context.Payments
            .AsNoTracking()
            .Where(p => p.TransactionTime >= startDate && p.TransactionTime <= endDate);

        // Apply terminal filter if provided
        if (!string.IsNullOrWhiteSpace(terminalFilter))
        {
            query = query.Where(p => p.TerminalId.ToString().Contains(terminalFilter));
        }

        // Execute query and group by payment type
        var payments = await query
            .Select(p => new
            {
                p.PaymentType,
                CardType = p is CreditCardPayment ? ((CreditCardPayment)p).CardType : 
                           p is DebitCardPayment ? ((DebitCardPayment)p).CardType : null,
                p.Amount.Amount,
                TipAmount = p.TipsAmount.Amount,
                p.TransactionType
            })
            .ToListAsync(cancellationToken);

        // Group payments by type and subtype
        var groupedPayments = payments
            .GroupBy(p => new { p.PaymentType, SubType = p.CardType ?? string.Empty })
            .Select(g => new PaymentTypeTotalDto
            {
                PaymentType = GetPaymentTypeDisplayName(g.Key.PaymentType),
                SubType = g.Key.SubType,
                TransactionCount = g.Count(),
                TotalAmount = g.Sum(p => p.Amount),
                TipAmount = g.Sum(p => p.TipAmount)
            })
            .OrderBy(g => g.PaymentType)
            .ThenBy(g => g.SubType)
            .ToList();

        report.PaymentTypes = groupedPayments;

        // Calculate totals by payment type
        report.Totals.TotalTransactions = payments.Count;
        report.Totals.TotalCash = payments.Where(p => p.PaymentType == PaymentType.Cash).Sum(p => p.Amount);
        report.Totals.TotalCreditCards = payments.Where(p => p.PaymentType == PaymentType.CreditCard || 
                                                           p.PaymentType == PaymentType.CreditVisa || 
                                                           p.PaymentType == PaymentType.CreditMasterCard || 
                                                           p.PaymentType == PaymentType.CreditAmex || 
                                                           p.PaymentType == PaymentType.CreditDiscover).Sum(p => p.Amount);
        report.Totals.TotalDebitCards = payments.Where(p => p.PaymentType == PaymentType.DebitCard || 
                                                          p.PaymentType == PaymentType.DebitVisa || 
                                                          p.PaymentType == PaymentType.DebitMasterCard).Sum(p => p.Amount);
        report.Totals.TotalGiftCertificates = payments.Where(p => p.PaymentType == PaymentType.GiftCertificate).Sum(p => p.Amount);
        report.Totals.TotalHouseAccounts = 0; // Not supported in current model
        report.Totals.TotalChecks = 0; // Not supported in current model
        report.Totals.TotalOther = payments.Where(p => p.PaymentType == PaymentType.CustomPayment).Sum(p => p.Amount);
        report.Totals.TotalTips = payments.Sum(p => p.TipAmount);

        return report;
    }

    private static string GetPaymentTypeDisplayName(PaymentType paymentType)
    {
        return paymentType switch
        {
            PaymentType.Cash => "Cash",
            PaymentType.CreditCard => "Credit Card",
            PaymentType.DebitCard => "Debit Card",
            PaymentType.GiftCertificate => "Gift Certificate",
            PaymentType.CustomPayment => "Custom Payment",
            _ => "Unknown"
        };
    }

    public async Task<MenuUsageReportDto> GetMenuUsageReportAsync(DateTime startDate, DateTime endDate, string? categoryFilter = null, string? orderTypeFilter = null, CancellationToken cancellationToken = default)
    {
        startDate = ToUtc(startDate);
        endDate = ToUtc(endDate);
        var report = new MenuUsageReportDto
        {
            StartDate = ToSafeDisplayDate(startDate),
            EndDate = ToSafeDisplayDate(endDate)
        };

        // Build base query for order lines in date range
        var query = from ol in _context.OrderLines
                    join t in _context.Tickets on ol.TicketId equals t.Id
                    where t.ClosedAt >= startDate && t.ClosedAt <= endDate && t.Status == TicketStatus.Closed
                    select ol;

        // Apply category filter if provided
        if (!string.IsNullOrWhiteSpace(categoryFilter))
        {
            query = query.Where(ol => ol.CategoryName.Contains(categoryFilter));
        }

        // Execute query and group by menu item in memory for simplicity or use efficient projection
        var data = await query
            .Select(ol => new
            {
                ol.MenuItemName,
                ol.CategoryName,
                ol.GroupName,
                ol.Quantity,
                TotalAmount = ol.TotalAmount.Amount,
                ol.TicketId
            })
            .ToListAsync(cancellationToken);

        // Group by menu item
        var groupedItems = data
            .GroupBy(ol => new { Name = ol.MenuItemName, CategoryName = ol.CategoryName ?? "Uncategorized", GroupName = ol.GroupName ?? "No Group" })
            .Select(g => new MenuUsageItemDto
            {
                ItemName = g.Key.Name,
                CategoryName = g.Key.CategoryName,
                GroupName = g.Key.GroupName,
                QuantitySold = (int)g.Sum(ol => ol.Quantity),
                Revenue = g.Sum(ol => ol.TotalAmount),
                TicketCount = g.Select(ol => ol.TicketId).Distinct().Count(),
                AveragePrice = g.Sum(ol => ol.Quantity) > 0 ? g.Sum(ol => ol.TotalAmount) / g.Sum(ol => ol.Quantity) : 0
            })
            .OrderByDescending(g => g.QuantitySold)
            .ToList();

        // Calculate percentages
        var totalRevenue = groupedItems.Sum(g => g.Revenue);
        foreach (var item in groupedItems)
        {
            item.PercentageOfTotal = totalRevenue > 0 ? (item.Revenue / totalRevenue) * 100 : 0;
        }

        report.Items = groupedItems;

        // Calculate totals
        report.Totals.TotalItems = groupedItems.Count;
        report.Totals.TotalQuantitySold = groupedItems.Sum(g => g.QuantitySold);
        report.Totals.TotalRevenue = totalRevenue;
        report.Totals.AverageQuantityPerItem = groupedItems.Count > 0 ? report.Totals.TotalQuantitySold / groupedItems.Count : 0;
        report.Totals.AverageRevenuePerItem = groupedItems.Count > 0 ? report.Totals.TotalRevenue / groupedItems.Count : 0;

        return report;
    }

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
                TotalSales = GetSalesForHour(h.Key, startDate, endDate),
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

    private decimal GetSalesForHour(int hour, DateTime startDate, DateTime endDate)
    {
        // Simple implementation: get sales for the specified hour across the date range
        // In a real implementation, this would be more sophisticated
        var hourStart = startDate.Date.AddHours(hour);
        var hourEnd = hourStart.AddHours(1);
        
        return _context.Tickets
            .AsNoTracking()
            .Where(t => t.ClosedAt >= hourStart && t.ClosedAt < hourEnd && t.Status == TicketStatus.Closed)
            .Sum(t => t.TotalAmount.Amount);
    }

    public async Task<DeliveryReportDto> GetDeliveryReportAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        startDate = ToUtc(startDate);
        endDate = ToUtc(endDate);
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
            PeriodStart = ToSafeDisplayDate(startDate),
            PeriodEnd = ToSafeDisplayDate(endDate)
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

    public async Task<CashOutReportDto> GetCashOutReportAsync(DateTime startDate, DateTime endDate, Guid? userIdFilter = null, CancellationToken cancellationToken = default)
    {
        startDate = ToUtc(startDate);
        endDate = ToUtc(endDate);

        var report = new CashOutReportDto
        {
            StartDate = ToSafeDisplayDate(startDate),
            EndDate = ToSafeDisplayDate(endDate)
        };

        // 1. Fetch relevant tickets (closed in range)
        var ticketsQuery = _context.Tickets
            .AsNoTracking()
            .Include(t => t.Payments) // Include payments to calculate cash/tips
            .Include(t => t.Gratuity) // Include gratuity for auto-grats
            .Where(t => t.ClosedAt >= startDate && t.ClosedAt <= endDate && t.Status == TicketStatus.Closed);

        if (userIdFilter.HasValue)
        {
             var uid = new UserId(userIdFilter.Value);
             // Should we filter by Ticket Creator? Or Server? Often they are the same.
             // Usually reports are "By Server".
             // Ticket.CreatedBy or Ticket.ServerId (if it exists)
             // Using CreatedBy as primary server
             ticketsQuery = ticketsQuery.Where(t => t.CreatedBy == uid);
        }

        var tickets = await ticketsQuery.ToListAsync(cancellationToken);

        // 2. Aggregate per User
        // Need to group by User.
        // If filtering by user, we still group (will be 1 group).
        
        var userIds = tickets.Select(t => t.CreatedBy.Value).Distinct().ToList();
        var users = await _context.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.FirstName + " " + u.LastName, cancellationToken);

        var reportItems = new List<CashOutReportItemDto>();

        foreach (var userId in userIds)
        {
            var userTickets = tickets.Where(t => t.CreatedBy.Value == userId).ToList();
            
            decimal cashSales = 0;
            decimal chargedTips = 0;
            
            foreach (var t in userTickets)
            {
                // A. Cash Sales: Sum of Cash Payments collected
                // IMPORTANT: Refunds (Negative) should reduce cash collected if refunded in cash
                // But usually we track "Cash in Hand".
                // If I refunded cash, I gave out cash.
                // Cash Payment (Credit) = Money In.
                // Cash Payment (Debit) = Money Out.
                // So Sum(Amount) where paymentType=Cash is correct net cash flow.
                
                var cashInHand = t.Payments
                    .Where(p => p.PaymentType == PaymentType.Cash)
                    .Sum(p => p.TransactionType == TransactionType.Credit ? p.Amount.Amount : -p.Amount.Amount);
                
                cashSales += cashInHand;

                // B. Charged Tips: Tips on Non-Cash payments
                // Also AutoGratuity on Non-Cash payments?
                // Floreant usually treats "Tips" as money owed to server.
                // If paid by Credit Card, house collected it, owes it to server.
                // If paid by Cash, server already has it (part of Gross Receipt), but usually we separate "Tip" from "Sales".
                // Let's assume Standard Model:
                // Net Due = (Cash Sales + Cash Tips) - (Credit Card Tips + Auto Grat on Card) -> Wait.
                // Simpler:
                // Server has wallet.
                // Wallet increases by Cash Payments (Sales + Cash Tips).
                // Wallet does NOT increase by CC Payments.
                // Server OWES House for Sales.
                // House OWES Server for CC Tips.
                // Net Due (Server -> House) = Cash In Wallet - Tips Earned (that house holds).
                // Cash In Wallet = Total Cash Payments.
                // Tips Earned (Held by House) = CC Tips.
                // Net Due = Cash Payments - CC Tips.
                
                var ccTips = t.Payments
                    .Where(p => p.PaymentType != PaymentType.Cash)
                    .Sum(p => p.TipsAmount.Amount);
                   
                // What about AutoGratuity?
                // If AutoGrat is on a CC ticket, it's like a Tip.
                // If AutoGrat is on a Cash ticket, it's collected in Cash.
                // Magidesk "Payments" might include tip/grat in Amount or separate?
                // Usually Payment.Amount includes Tip.
                // So Cash Payment = Subtotal + Tax + Tip.
                // So "Cash In Wallet" already includes Cash Tips.
                // "CC Tips" is just the tip part.
                // AutoGratuity logic in Magidesk might be separate entity 'Gratuity'.
                // If Gratuity exists, it is expected to be paid.
                // If Paid by Cash, it's in Cash Payment.
                // If Paid by CC, it's in CC Payment.
                // We need to know if the House owes the server the AutoGrat.
                // Yes, if collected via CC.
                // How do we know if AutoGrat was paid by CC?
                // We look at payments.
                // Proportionally? Or just sum all "Non-Cash Tips/Grat"?
                
                // Refinment: AutoGratuity is distinct from "TipsAmount" on Payment in some models.
                // In Magidesk, Ticket.Gratuity is independent.
                // Let's assume standard CC Tips for now as primary "Owed to Server".
                // If we need strict parity with Floreant's complex auto-grat, we might need deeper inspection.
                // Use Standard: Charged Tips = Sum of Tips on Non-Cash Payments.
                
                chargedTips += ccTips;
            }

            var item = new CashOutReportItemDto
            {
                UserId = userId,
                UserName = users.GetValueOrDefault(userId, "Unknown"),
                TicketCount = userTickets.Count,
                CashSales = cashSales,
                ChargedTips = chargedTips
            };
            reportItems.Add(item);
        }

        report.Items = reportItems;
        report.TotalCashSales = reportItems.Sum(i => i.CashSales);
        report.TotalChargedTips = reportItems.Sum(i => i.ChargedTips);
        report.TotalNetDue = reportItems.Sum(i => i.NetDue);

        return report;
    }
}
