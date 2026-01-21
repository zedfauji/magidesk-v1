using System.Threading;
using System.Threading.Tasks;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Interfaces;

public interface ISystemService
{
    /// <summary>
    /// Performs a graceful system shutdown, ensuring all critical logs are flushed.
    /// </summary>
    /// <param name="initiatedBy">The user initiating the shutdown.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ShutdownAsync(UserId initiatedBy, CancellationToken cancellationToken = default);
}
