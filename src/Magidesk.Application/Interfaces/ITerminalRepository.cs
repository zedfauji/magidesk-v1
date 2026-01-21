using System.Threading;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface ITerminalRepository
{
    Task<Terminal?> GetByTerminalKeyAsync(string terminalKey, CancellationToken cancellationToken = default);
    Task UpdateTerminalAsync(Terminal terminal, CancellationToken cancellationToken = default);
    Task AddTerminalAsync(Terminal terminal, CancellationToken cancellationToken = default);
}
