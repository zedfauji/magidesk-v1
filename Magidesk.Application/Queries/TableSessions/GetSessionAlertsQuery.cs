using System;
using System.Collections.Generic;
using Magidesk.Application.DTOs;

namespace Magidesk.Application.Queries.TableSessions;

/// <summary>
/// Query to get session alerts for monitoring dashboard.
/// </summary>
public record GetSessionAlertsQuery();

/// <summary>
/// Query to get alerts for a specific session.
/// </summary>
public record GetSessionAlertsForSessionQuery(Guid SessionId);