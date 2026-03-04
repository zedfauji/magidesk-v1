using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.Commands.TableSessions;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.ViewModels.Dialogs;
using Magidesk.Presentation.Views.Dialogs;
using Magidesk.Presentation.ViewModels;
using Magidesk.Presentation.Views;
using Magidesk.Presentation.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.Commands.TableSessions;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.ViewModels.Dialogs;
using Magidesk.Presentation.Views.Dialogs;
using Magidesk.Presentation.ViewModels;
using Magidesk.Presentation.Views;
using Magidesk.Presentation.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using System.Collections.ObjectModel;

using Magidesk.Domain.Enumerations;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for computed properties and read-only properties.
/// </summary>
public partial class OrderPageViewModel
{
    // Ticket Information
    public string TicketNumber => _ticket != null ? $"Ticket #{_ticket.TicketNumber}" : "New Order";

    public DateTime TicketStartTime => _ticket?.CreatedAt ?? DateTime.Now;

    public TimeSpan WaitTime => DateTime.Now - TicketStartTime;

    public bool HasTicket => _ticketId.HasValue && _ticket != null;

    // Session Information
    public string TerminalName => _terminalContext.TerminalIdentity ?? "Terminal";

    public string UserName => _userService.CurrentUser?.FullName ?? "User";

    public string SystemStatus => "ONLINE"; // TODO: Implement actual status check

    public DateTime CurrentTime => DateTime.Now;

    // Session State
    public SessionState CurrentSessionState
    {
        get
        {
            if (_ticket?.SessionStatus == null || !_ticket.HasActiveSession)
                return SessionState.NotStarted;

            return _ticket.SessionStatus switch
            {
                TableSessionStatus.Active => SessionState.Active,
                TableSessionStatus.Paused => SessionState.Paused,
                _ => SessionState.NotStarted
            };
        }
    }

    public bool IsSessionActive => CurrentSessionState == SessionState.Active;

    public bool IsSessionPaused => CurrentSessionState == SessionState.Paused;

    public string SessionButtonText => CurrentSessionState switch
    {
        SessionState.NotStarted => "Start Session",
        SessionState.Active => "Pause Session",
        SessionState.Paused => "Resume Session",
        _ => "Start Session"
    };

    public bool IsEndSessionEnabled => CurrentSessionState == SessionState.Active || CurrentSessionState == SessionState.Paused;

    public string SessionDurationDisplay
    {
        get
        {
            if (_ticket?.SessionElapsedTime == null)
                return "00:00:00";

            var duration = _ticket.SessionElapsedTime.Value;
            return $"{(int)duration.TotalHours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}";
        }
    }

    // Statistics
    public int TotalItemCount => OrderItems.Sum(item => item.Quantity);
}
