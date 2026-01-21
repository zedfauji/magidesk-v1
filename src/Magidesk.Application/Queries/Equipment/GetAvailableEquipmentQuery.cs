using System;
using System.Collections.Generic;
using Magidesk.Application.DTOs;

namespace Magidesk.Application.Queries.Equipment;

/// <summary>
/// Query to get available equipment that can be assigned to tables.
/// </summary>
public record GetAvailableEquipmentQuery();

/// <summary>
/// Query to get equipment assigned to a specific table.
/// </summary>
public record GetTableEquipmentQuery(Guid TableId);