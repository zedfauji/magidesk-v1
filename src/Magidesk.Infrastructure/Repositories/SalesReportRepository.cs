using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Enumerations;
using Magidesk.Infrastructure.Data;
using Magidesk.Domain.ValueObjects;
using Magidesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

public partial class SalesReportRepository : ISalesReportRepository
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

}
