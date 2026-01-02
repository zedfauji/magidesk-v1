using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface IPrinterGroupRepository
{
    Task<IEnumerable<PrinterGroup>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PrinterGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(PrinterGroup group, CancellationToken cancellationToken = default);
    Task UpdateAsync(PrinterGroup group, CancellationToken cancellationToken = default);
    Task DeleteAsync(PrinterGroup group, CancellationToken cancellationToken = default);
}
