using System;
using System.Threading.Tasks;

namespace Magidesk.Application.Interfaces;

public interface ITicketCreationService
{
    /// <summary>
    /// Creates a new ticket for a specific table, handling all validation and context checks.
    /// </summary>
    /// <param name="tableId">The ID of the table to assign.</param>
    /// <param name="userId">The ID of the user creating the ticket.</param>
    /// <returns>The ID of the newly created ticket.</returns>
    Task<Guid> CreateTicketForTableAsync(Guid tableId, Guid userId);
}
