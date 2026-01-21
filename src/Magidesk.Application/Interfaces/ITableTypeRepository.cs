using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Repository interface for TableType entity.
/// </summary>
public interface ITableTypeRepository
{
    /// <summary>
    /// Gets a table type by ID.
    /// </summary>
    Task<TableType?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets all table types.
    /// </summary>
    /// <param name="includeInactive">Include inactive table types</param>
    Task<IEnumerable<TableType>> GetAllAsync(bool includeInactive = false);

    /// <summary>
    /// Gets a table type by name.
    /// </summary>
    Task<TableType?> GetByNameAsync(string name);

    /// <summary>
    /// Adds a new table type.
    /// </summary>
    Task<TableType> AddAsync(TableType tableType);

    /// <summary>
    /// Updates an existing table type.
    /// </summary>
    Task UpdateAsync(TableType tableType);

    /// <summary>
    /// Deletes a table type.
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Checks if a table type with the given name exists.
    /// </summary>
    Task<bool> ExistsAsync(string name, Guid? excludeId = null);
}
