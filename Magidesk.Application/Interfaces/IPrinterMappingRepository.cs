using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface IPrinterMappingRepository
{
    Task<IEnumerable<PrinterMapping>> GetByTerminalIdAsync(Guid terminalId, CancellationToken cancellationToken = default);
    Task AddAsync(PrinterMapping mapping, CancellationToken cancellationToken = default);
    Task UpdateAsync(PrinterMapping mapping, CancellationToken cancellationToken = default);
    Task DeleteAsync(PrinterMapping mapping, CancellationToken cancellationToken = default);
}
