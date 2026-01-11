using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Magidesk.ViewModels;

/// <summary>
/// ViewModel for gratuity selection dialog.
/// Provides suggested tip amounts and custom entry with real-time calculation.
/// </summary>
public partial class GratuitySelectionViewModel : ObservableObject
{
    private readonly IGratuityService _gratuityService;
    private readonly ICommandHandler<ApplyGratuityCommand, ApplyGratuityResult> _applyGratuityHandler;
    private readonly IDialogService _dialogService;
    private readonly ILogger<GratuitySelectionViewModel> _logger;
    private readonly Guid _ticketId;
    private readonly Money _subtotal;
    private readonly UserId _currentUserId;

    [ObservableProperty]
    private string _ticketNumber = string.Empty;

    [ObservableProperty]
    private string _subtotalDisplay = string.Empty;

    [ObservableProperty]
    private ObservableCollection<GratuitySuggestionItem> _suggestions = new();

    [ObservableProperty]
    private GratuitySuggestionItem? _selectedSuggestion;

    [ObservableProperty]
    private string _customAmount = string.Empty;

    [ObservableProperty]
    private bool _isCustomAmountMode;

    [ObservableProperty]
    private string _totalWithGratuityDisplay = string.Empty;

    [ObservableProperty]
    private bool _isProcessing;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private ObservableCollection<ServerItem> _availableServers = new();

    [ObservableProperty]
    private ServerItem? _selectedServer;

    public GratuitySelectionViewModel(
        IGratuityService gratuityService,
        ICommandHandler<ApplyGratuityCommand, ApplyGratuityResult> applyGratuityHandler,
        IDialogService dialogService,
        ILogger<GratuitySelectionViewModel> logger,
        Guid ticketId,
        string ticketNumber,
        Money subtotal,
        UserId currentUserId,
        ObservableCollection<ServerItem> availableServers)
    {
        _gratuityService = gratuityService ?? throw new ArgumentNullException(nameof(gratuityService));
        _applyGratuityHandler = applyGratuityHandler ?? throw new ArgumentNullException(nameof(applyGratuityHandler));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _ticketId = ticketId;
        _subtotal = subtotal ?? throw new ArgumentNullException(nameof(subtotal));
        _currentUserId = currentUserId ?? throw new ArgumentNullException(nameof(currentUserId));

        TicketNumber = ticketNumber;
        SubtotalDisplay = subtotal.ToString();
        AvailableServers = availableServers ?? new ObservableCollection<ServerItem>();

        // Set default server to current user
        SelectedServer = AvailableServers.FirstOrDefault(s => s.UserId == currentUserId);

        LoadSuggestions();
    }

    private void LoadSuggestions()
    {
        try
        {
            var suggestions = _gratuityService.GetSuggestions(_subtotal);

            Suggestions = new ObservableCollection<GratuitySuggestionItem>
            {
                new GratuitySuggestionItem("15%", suggestions.Percent15, 0.15m),
                new GratuitySuggestionItem("18%", suggestions.Percent18, 0.18m),
                new GratuitySuggestionItem("20%", suggestions.Percent20, 0.20m),
                new GratuitySuggestionItem("25%", suggestions.Percent25, 0.25m)
            };

            // Pre-select 18% as default
            SelectedSuggestion = Suggestions.FirstOrDefault(s => s.Percentage == 0.18m);
            UpdateTotalDisplay();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading gratuity suggestions for ticket {TicketId}", _ticketId);
            ShowError("Failed to load tip suggestions.");
        }
    }

    partial void OnSelectedSuggestionChanged(GratuitySuggestionItem? value)
    {
        if (value != null)
        {
            IsCustomAmountMode = false;
            CustomAmount = string.Empty;
            UpdateTotalDisplay();
        }
    }

    partial void OnCustomAmountChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            IsCustomAmountMode = true;
            SelectedSuggestion = null;
            UpdateTotalDisplay();
        }
    }

    private void UpdateTotalDisplay()
    {
        try
        {
            Money gratuityAmount;

            if (IsCustomAmountMode && decimal.TryParse(CustomAmount, out var customValue))
            {
                gratuityAmount = new Money(customValue);
            }
            else if (SelectedSuggestion != null)
            {
                gratuityAmount = SelectedSuggestion.Amount;
            }
            else
            {
                TotalWithGratuityDisplay = _subtotal.ToString();
                return;
            }

            var total = _subtotal + gratuityAmount;
            TotalWithGratuityDisplay = total.ToString();
        }
        catch
        {
            TotalWithGratuityDisplay = _subtotal.ToString();
        }
    }

    [RelayCommand]
    private async Task ApplyGratuityAsync()
    {
        try
        {
            IsProcessing = true;
            ClearError();

            // Validate amount
            Money gratuityAmount;
            if (IsCustomAmountMode)
            {
                if (!decimal.TryParse(CustomAmount, out var customValue) || customValue <= 0)
                {
                    ShowError("Please enter a valid tip amount.");
                    return;
                }
                gratuityAmount = new Money(customValue);
            }
            else if (SelectedSuggestion != null)
            {
                gratuityAmount = SelectedSuggestion.Amount;
            }
            else
            {
                ShowError("Please select a tip amount or enter a custom amount.");
                return;
            }

            // Validate server selection
            if (SelectedServer == null)
            {
                ShowError("Please select a server for tip assignment.");
                return;
            }

            // Apply gratuity
            var command = new ApplyGratuityCommand
            {
                TicketId = _ticketId,
                Amount = gratuityAmount,
                ServerId = SelectedServer.UserId,
                ProcessedBy = _currentUserId
            };

            var result = await _applyGratuityHandler.HandleAsync(command);

            if (!result.Success)
            {
                ShowError(result.ErrorMessage ?? "Failed to apply gratuity.");
                return;
            }

            _logger.LogInformation(
                "Gratuity {Amount} applied to ticket {TicketNumber} for server {ServerName}",
                gratuityAmount,
                TicketNumber,
                SelectedServer.DisplayName);

            // Dialog will close via ContentDialog result
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying gratuity to ticket {TicketId}", _ticketId);
            ShowError($"Failed to apply gratuity: {ex.Message}");
        }
        finally
        {
            IsProcessing = false;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        // Dialog will close via ContentDialog result
    }

    [RelayCommand]
    private void SelectCustomAmount()
    {
        IsCustomAmountMode = true;
        SelectedSuggestion = null;
        CustomAmount = string.Empty;
    }

    private void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = true;
    }

    private void ClearError()
    {
        ErrorMessage = null;
        HasError = false;
    }
}

/// <summary>
/// Represents a suggested gratuity amount.
/// </summary>
public class GratuitySuggestionItem
{
    public string Label { get; }
    public Money Amount { get; }
    public decimal Percentage { get; }
    public string DisplayText => $"{Label}\n{Amount}";

    public GratuitySuggestionItem(string label, Money amount, decimal percentage)
    {
        Label = label;
        Amount = amount;
        Percentage = percentage;
    }
}

/// <summary>
/// Represents a server/staff member for tip assignment.
/// </summary>
public class ServerItem
{
    public UserId UserId { get; }
    public string DisplayName { get; }
    public string? Role { get; }

    public ServerItem(UserId userId, string displayName, string? role = null)
    {
        UserId = userId;
        DisplayName = displayName;
        Role = role;
    }
}
