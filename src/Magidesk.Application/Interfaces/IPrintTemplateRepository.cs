using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface IPrintTemplateRepository : IRepository<PrintTemplate>
{
    Task<PrintTemplate?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<List<PrintTemplate>> GetSystemTemplatesAsync(CancellationToken cancellationToken);
}
