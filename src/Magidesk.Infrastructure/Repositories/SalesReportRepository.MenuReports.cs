using Magidesk.Application.DTOs.Reports;
using Magidesk.Domain.Enumerations;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

public partial class SalesReportRepository
{
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
}
