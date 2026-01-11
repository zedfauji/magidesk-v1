using System;
using System.Collections.Generic;
using Magidesk.Application.DTOs;

namespace Magidesk.Application.Queries;

/// <summary>
/// Query for searching customers with pagination.
/// </summary>
public record SearchCustomersQuery(
    string SearchTerm,
    int PageNumber = 1,
    int PageSize = 20
);
