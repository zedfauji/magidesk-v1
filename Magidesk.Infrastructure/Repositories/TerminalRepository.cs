using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

public class TerminalRepository : ITerminalRepository
{
    private readonly ApplicationDbContext _context;

    public TerminalRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Terminal?> GetByTerminalKeyAsync(string terminalKey, CancellationToken cancellationToken = default)
    {
        return await _context.Terminals
            .FirstOrDefaultAsync(t => t.TerminalKey == terminalKey, cancellationToken);
    }

    public async Task AddTerminalAsync(Terminal terminal, CancellationToken cancellationToken = default)
    {
        await _context.Terminals.AddAsync(terminal, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateTerminalAsync(Terminal terminal, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Terminals.FindAsync(new object[] { terminal.Id }, cancellationToken);
        if (existing != null)
        {
            existing.Name = terminal.Name;
            existing.Location = terminal.Location;
            existing.FloorId = terminal.FloorId;
            existing.HasCashDrawer = terminal.HasCashDrawer;
            existing.OpeningBalance = terminal.OpeningBalance;
            existing.CurrentBalance = terminal.CurrentBalance;
            existing.AutoLogOut = terminal.AutoLogOut;
            existing.AutoLogOutTime = terminal.AutoLogOutTime;
            existing.ShowGuestSelection = terminal.ShowGuestSelection;
            existing.ShowTableSelection = terminal.ShowTableSelection;
            existing.KitchenMode = terminal.KitchenMode;
            existing.DefaultFontSize = terminal.DefaultFontSize;
            existing.DefaultFontFamily = terminal.DefaultFontFamily;

            _context.Terminals.Update(existing);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
