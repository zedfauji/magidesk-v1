# FE-C.2-01: Hold Ticket UI - Detailed Implementation

**Ticket ID:** FE-C.2-01  
**Feature ID:** C.2  
**Title:** Hold Ticket UI  
**Priority:** P0  
**Status:** READY FOR IMPLEMENTATION  
**Dependencies**: BE-C.2-01

---

## Overview

Implement the user interface for holding tickets (deferred payment/tabs), allowing staff to hold a ticket for later payment while freeing up the table.

---

## Technical Design

### 1. ViewModels

#### 1.1 Hold Ticket Dialog ViewModel
**File**: `ViewModels/Dialogs/HoldTicketDialogViewModel.cs`

```csharp
namespace Magidesk.Presentation.ViewModels.Dialogs;

public partial class HoldTicketDialogViewModel : ObservableObject
{
    private readonly IMediator _mediator;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private Guid _ticketId;

    [ObservableProperty]
    private string _ticketNumber = string.Empty;

    [ObservableProperty]
    private decimal _totalAmount;

    [ObservableProperty]
    private string _holdReason = string.Empty;

    [ObservableProperty]
    private string _selectedReasonCode = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    public ObservableCollection<string> ReasonCodes { get; } = new()
    {
        "Customer Tab",
        "Charge to Room",
        "Deferred Payment",
        "Manager Approval Pending",
        "Other"
    };

    public HoldTicketDialogViewModel(IMediator mediator, IDialogService dialogService)
    {
        _mediator = mediator;
        _dialogService = dialogService;
    }

    public void Initialize(Guid ticketId, string ticketNumber, decimal totalAmount)
    {
        TicketId = ticketId;
        TicketNumber = ticketNumber;
        TotalAmount = totalAmount;
        SelectedReasonCode = ReasonCodes[0];
    }

    [RelayCommand]
    private async Task HoldTicketAsync()
    {
        if (string.IsNullOrWhiteSpace(HoldReason) && SelectedReasonCode == "Other")
        {
            HasError = true;
            ErrorMessage = "Please provide a reason for holding the ticket.";
            return;
        }

        IsLoading = true;
        HasError = false;

        try
        {
            var reason = SelectedReasonCode == "Other" ? HoldReason : SelectedReasonCode;
            var command = new HoldTicketCommand(TicketId, reason, App.CurrentUser.Id);
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                await _dialogService.ShowSuccessAsync("Ticket Held", "Ticket has been held for later payment.");
                // Close dialog with success
            }
            else
            {
                HasError = true;
                ErrorMessage = result.Error;
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = $"Failed to hold ticket: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        // Close dialog
    }
}
```

#### 1.2 Held Tickets ViewModel
**File**: `ViewModels/HeldTicketsViewModel.cs`

```csharp
namespace Magidesk.Presentation.ViewModels;

public partial class HeldTicketsViewModel : ObservableObject
{
    private readonly IMediator _mediator;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<HeldTicketDto> _heldTickets = new();

    [ObservableProperty]
    private HeldTicketDto? _selectedTicket;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string _searchText = string.Empty;

    public HeldTicketsViewModel(IMediator mediator, IDialogService dialogService)
    {
        _mediator = mediator;
        _dialogService = dialogService;
    }

    public async Task LoadAsync()
    {
        IsLoading = true;
        HasError = false;

        try
        {
            var query = new GetHeldTicketsQuery();
            var result = await _mediator.Send(query);

            if (result.IsSuccess)
            {
                HeldTickets = new ObservableCollection<HeldTicketDto>(result.Value);
            }
            else
            {
                HasError = true;
                ErrorMessage = result.Error;
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = $"Failed to load held tickets: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ReleaseTicketAsync(HeldTicketDto ticket)
    {
        var confirmed = await _dialogService.ShowConfirmationAsync(
            "Release Ticket",
            $"Release ticket {ticket.TicketNumber} for payment?",
            "Release",
            "Cancel");

        if (!confirmed)
            return;

        IsLoading = true;

        try
        {
            var command = new ReleaseHeldTicketCommand(ticket.Id, App.CurrentUser.Id);
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                await _dialogService.ShowSuccessAsync("Ticket Released", "Ticket is now ready for payment.");
                await LoadAsync(); // Refresh list
            }
            else
            {
                await _dialogService.ShowErrorAsync("Error", result.Error);
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Error", $"Failed to release ticket: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ViewTicketDetailsAsync(HeldTicketDto ticket)
    {
        // Navigate to ticket details or open settle page
        // Implementation depends on navigation service
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadAsync();
    }

    partial void OnSearchTextChanged(string value)
    {
        // Implement filtering logic
        FilterTickets();
    }

    private void FilterTickets()
    {
        // Filter held tickets based on search text
        // Implementation depends on requirements
    }
}
```

---

### 2. Views

#### 2.1 Hold Ticket Dialog
**File**: `Views/Dialogs/HoldTicketDialog.xaml`

```xml
<ContentDialog
    x:Class="Magidesk.Presentation.Views.Dialogs.HoldTicketDialog"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d"
    Title="Hold Ticket"
    PrimaryButtonText="Hold Ticket"
    CloseButtonText="Cancel"
    DefaultButton="Primary"
    PrimaryButtonCommand="{x:Bind ViewModel.HoldTicketCommand}"
    Style="{StaticResource DefaultContentDialogStyle}">

    <StackPanel Spacing="16" MinWidth="400">
        <!-- Ticket Info -->
        <StackPanel Spacing="8">
            <TextBlock Text="Ticket Information" Style="{StaticResource SubtitleTextBlockStyle}" />
            
            <Grid ColumnSpacing="12">
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="Auto" />
                    <ColumnDefinition Width="*" />
                </Grid.ColumnDefinitions>
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto" />
                    <RowDefinition Height="Auto" />
                </Grid.RowDefinitions>

                <TextBlock Grid.Row="0" Grid.Column="0" Text="Ticket #:" />
                <TextBlock Grid.Row="0" Grid.Column="1" Text="{x:Bind ViewModel.TicketNumber, Mode=OneWay}" FontWeight="SemiBold" />

                <TextBlock Grid.Row="1" Grid.Column="0" Text="Total:" />
                <TextBlock Grid.Row="1" Grid.Column="1" 
                           Text="{x:Bind ViewModel.TotalAmount, Mode=OneWay, Converter={StaticResource CurrencyConverter}}" 
                           FontWeight="SemiBold" />
            </Grid>
        </StackPanel>

        <!-- Reason Selection -->
        <StackPanel Spacing="8">
            <TextBlock Text="Hold Reason" Style="{StaticResource SubtitleTextBlockStyle}" />
            
            <ComboBox 
                Header="Select Reason"
                ItemsSource="{x:Bind ViewModel.ReasonCodes}"
                SelectedItem="{x:Bind ViewModel.SelectedReasonCode, Mode=TwoWay}"
                HorizontalAlignment="Stretch" />

            <!-- Custom Reason (shown when "Other" selected) -->
            <TextBox 
                Header="Custom Reason"
                Text="{x:Bind ViewModel.HoldReason, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
                PlaceholderText="Enter reason for holding ticket"
                MaxLength="500"
                Visibility="{x:Bind ViewModel.SelectedReasonCode, Mode=OneWay, Converter={StaticResource EqualToVisibilityConverter}, ConverterParameter='Other'}"
                HorizontalAlignment="Stretch" />
        </StackPanel>

        <!-- Warning Message -->
        <InfoBar
            Severity="Warning"
            IsOpen="True"
            IsClosable="False"
            Message="The table will be released and available for other customers. The ticket will remain open for later payment." />

        <!-- Error Message -->
        <InfoBar
            Severity="Error"
            IsOpen="{x:Bind ViewModel.HasError, Mode=OneWay}"
            Message="{x:Bind ViewModel.ErrorMessage, Mode=OneWay}" />

        <!-- Loading Indicator -->
        <ProgressRing IsActive="{x:Bind ViewModel.IsLoading, Mode=OneWay}" />
    </StackPanel>
</ContentDialog>
```

#### 2.2 Held Tickets Page
**File**: `Views/HeldTicketsPage.xaml`

```xml
<Page
    x:Class="Magidesk.Presentation.Views.HeldTicketsPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d">

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="*" />
        </Grid.RowDefinitions>

        <!-- Header -->
        <Grid Grid.Row="0" Padding="24,16" Background="{ThemeResource CardBackgroundFillColorDefaultBrush}">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*" />
                <ColumnDefinition Width="Auto" />
            </Grid.ColumnDefinitions>

            <StackPanel Grid.Column="0" Spacing="4">
                <TextBlock Text="Held Tickets" Style="{StaticResource TitleTextBlockStyle}" />
                <TextBlock Text="Tickets held for later payment" Style="{StaticResource CaptionTextBlockStyle}" Foreground="{ThemeResource TextFillColorSecondaryBrush}" />
            </StackPanel>

            <StackPanel Grid.Column="1" Orientation="Horizontal" Spacing="8">
                <AutoSuggestBox 
                    PlaceholderText="Search tickets..."
                    Text="{x:Bind ViewModel.SearchText, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
                    Width="250" />

                <Button Command="{x:Bind ViewModel.RefreshCommand}">
                    <StackPanel Orientation="Horizontal" Spacing="8">
                        <FontIcon Glyph="&#xE72C;" FontSize="16" />
                        <TextBlock Text="Refresh" />
                    </StackPanel>
                </Button>
            </StackPanel>
        </Grid>

        <!-- Content -->
        <Grid Grid.Row="1" Padding="24">
            <!-- Loading State -->
            <ProgressRing IsActive="{x:Bind ViewModel.IsLoading, Mode=OneWay}" 
                          Width="60" Height="60"
                          Visibility="{x:Bind ViewModel.IsLoading, Mode=OneWay, Converter={StaticResource BoolToVisibilityConverter}}" />

            <!-- Error State -->
            <InfoBar
                Severity="Error"
                IsOpen="{x:Bind ViewModel.HasError, Mode=OneWay}"
                Message="{x:Bind ViewModel.ErrorMessage, Mode=OneWay}"
                Visibility="{x:Bind ViewModel.HasError, Mode=OneWay, Converter={StaticResource BoolToVisibilityConverter}}" />

            <!-- Tickets List -->
            <ListView 
                ItemsSource="{x:Bind ViewModel.HeldTickets, Mode=OneWay}"
                SelectedItem="{x:Bind ViewModel.SelectedTicket, Mode=TwoWay}"
                SelectionMode="Single"
                Visibility="{x:Bind ViewModel.IsLoading, Mode=OneWay, Converter={StaticResource InverseBoolToVisibilityConverter}}">
                
                <ListView.ItemTemplate>
                    <DataTemplate x:DataType="local:HeldTicketDto">
                        <Grid Padding="16" ColumnSpacing="16">
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="Auto" />
                                <ColumnDefinition Width="*" />
                                <ColumnDefinition Width="Auto" />
                                <ColumnDefinition Width="Auto" />
                            </Grid.ColumnDefinitions>

                            <!-- Ticket Icon -->
                            <FontIcon Grid.Column="0" Glyph="&#xE8A1;" FontSize="32" Foreground="{ThemeResource AccentFillColorDefaultBrush}" />

                            <!-- Ticket Info -->
                            <StackPanel Grid.Column="1" Spacing="4">
                                <TextBlock Text="{x:Bind TicketNumber}" FontWeight="SemiBold" FontSize="16" />
                                <TextBlock Text="{x:Bind HoldReason}" Foreground="{ThemeResource TextFillColorSecondaryBrush}" />
                                <StackPanel Orientation="Horizontal" Spacing="8">
                                    <TextBlock Text="Held:" Foreground="{ThemeResource TextFillColorTertiaryBrush}" />
                                    <TextBlock Text="{x:Bind HeldAt, Converter={StaticResource DateTimeConverter}}" Foreground="{ThemeResource TextFillColorTertiaryBrush}" />
                                    <TextBlock Text="by" Foreground="{ThemeResource TextFillColorTertiaryBrush}" />
                                    <TextBlock Text="{x:Bind HeldByUserName}" Foreground="{ThemeResource TextFillColorTertiaryBrush}" />
                                </StackPanel>
                                <TextBlock Text="{x:Bind CustomerName}" Visibility="{x:Bind CustomerName, Converter={StaticResource NullToVisibilityConverter}}" />
                            </StackPanel>

                            <!-- Amount -->
                            <StackPanel Grid.Column="2" VerticalAlignment="Center">
                                <TextBlock Text="Total" Foreground="{ThemeResource TextFillColorSecondaryBrush}" FontSize="12" />
                                <TextBlock Text="{x:Bind TotalAmount, Converter={StaticResource CurrencyConverter}}" 
                                           FontWeight="SemiBold" FontSize="18" />
                            </StackPanel>

                            <!-- Actions -->
                            <StackPanel Grid.Column="3" Orientation="Horizontal" Spacing="8" VerticalAlignment="Center">
                                <Button Command="{Binding DataContext.ViewTicketDetailsCommand, ElementName=Page}" 
                                        CommandParameter="{x:Bind}"
                                        Style="{StaticResource AccentButtonStyle}">
                                    <StackPanel Orientation="Horizontal" Spacing="8">
                                        <FontIcon Glyph="&#xE8A1;" FontSize="16" />
                                        <TextBlock Text="View" />
                                    </StackPanel>
                                </Button>

                                <Button Command="{Binding DataContext.ReleaseTicketCommand, ElementName=Page}" 
                                        CommandParameter="{x:Bind}">
                                    <StackPanel Orientation="Horizontal" Spacing="8">
                                        <FontIcon Glyph="&#xE73E;" FontSize="16" />
                                        <TextBlock Text="Release" />
                                    </StackPanel>
                                </Button>
                            </StackPanel>
                        </Grid>
                    </DataTemplate>
                </ListView.ItemTemplate>
            </ListView>

            <!-- Empty State -->
            <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center" Spacing="16"
                        Visibility="{x:Bind ViewModel.HeldTickets.Count, Mode=OneWay, Converter={StaticResource ZeroToVisibilityConverter}}">
                <FontIcon Glyph="&#xE8A1;" FontSize="64" Foreground="{ThemeResource TextFillColorTertiaryBrush}" />
                <TextBlock Text="No Held Tickets" Style="{StaticResource SubtitleTextBlockStyle}" />
                <TextBlock Text="Tickets held for later payment will appear here" 
                           Foreground="{ThemeResource TextFillColorSecondaryBrush}" />
            </StackPanel>
        </Grid>
    </Grid>
</Page>
```

---

### 3. Integration Points

#### 3.1 Update SettlePage
Add "Hold Ticket" button to the SettlePage:

```xml
<Button Command="{x:Bind ViewModel.HoldTicketCommand}" Style="{StaticResource AccentButtonStyle}">
    <StackPanel Orientation="Horizontal" Spacing="8">
        <FontIcon Glyph="&#xE8A1;" />
        <TextBlock Text="Hold Ticket" />
    </StackPanel>
</Button>
```

#### 3.2 Update Navigation
Add navigation item for Held Tickets page in the main navigation menu.

---

## Acceptance Criteria

- [ ] "Hold Ticket" button available on SettlePage
- [ ] Hold Ticket dialog captures reason
- [ ] Held tickets page displays all held tickets
- [ ] Can release held ticket from list
- [ ] Table status updates when ticket held
- [ ] Visual feedback for success/error
- [ ] Search/filter functionality works
- [ ] Refresh button updates list

---

## Implementation Checklist

### ViewModels
- [ ] Create `HoldTicketDialogViewModel`
- [ ] Create `HeldTicketsViewModel`
- [ ] Add `HoldTicketCommand` to SettlePageViewModel

### Views
- [ ] Create `HoldTicketDialog.xaml`
- [ ] Create `HeldTicketsPage.xaml`
- [ ] Update `SettlePage.xaml` with Hold button

### Integration
- [ ] Register ViewModels in DI container
- [ ] Add navigation route for Held Tickets page
- [ ] Update main navigation menu

### Testing
- [ ] Manual testing of hold flow
- [ ] Manual testing of release flow
- [ ] UI/UX review

---

*Ready for implementation - January 14, 2026*
