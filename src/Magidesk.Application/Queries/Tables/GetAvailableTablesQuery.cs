using System;
using System.Collections.Generic;
using Magidesk.Application.DTOs;

namespace Magidesk.Application.Queries.Tables;

/// <summary>
/// Query to get available tables for operations (excluding specified table).
/// </summary>
public record GetAvailableTablesQuery(Guid? ExcludeTableId = null);