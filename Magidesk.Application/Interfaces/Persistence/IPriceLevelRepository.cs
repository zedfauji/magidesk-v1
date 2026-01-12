using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces.Persistence;

/// <summary>
/// Repository interface for managing PriceLevel entities.
/// </summary>
public interface IPriceLevelRepository : IRepository<PriceLevel>
{
    // specific methods can be added here if needed, e.g. finding by name, etc.
}
