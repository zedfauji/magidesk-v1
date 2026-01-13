using System;
using System.Collections.Generic;
using Magidesk.Application.DTOs;

namespace Magidesk.Application.Queries.TableTypes;

/// <summary>
/// Query to get all table types.
/// </summary>
public record GetTableTypesQuery();

/// <summary>
/// Query to get a specific table type by ID.
/// </summary>
public record GetTableTypeByIdQuery(Guid TableTypeId);