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

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for lifecycle management.
/// Handles initialization, cleanup, and resource disposal.
/// </summary>
public partial class OrderPageViewModel
{
    /// <summary>
    /// Sets the XamlRoot for dialogs. Must be called from the View after it's loaded.
    /// </summary>
    public void SetXamlRoot(Microsoft.UI.Xaml.XamlRoot xamlRoot)
    {
        _xamlRoot = xamlRoot;
    }

    /// <summary>
    /// Refreshes the current ticket data from the repository.
    /// Used when navigating back from SettlePageView to reload any changes.
    /// </summary>
    public async Task RefreshTicketAsync()
    {
        if (_ticketId.HasValue)
        {
            await LoadTicketAsync();
        }
    }

    /// <summary>
    /// Cleanup resources used by the ViewModel.
    /// Should be called when the view is being disposed.
    /// </summary>
    public void Cleanup()
    {
        _timeUpdateTimer?.Stop();
        _timeUpdateTimer?.Dispose();
        _sessionDurationTimer?.Stop();
        _sessionDurationTimer?.Dispose();
    }
}
