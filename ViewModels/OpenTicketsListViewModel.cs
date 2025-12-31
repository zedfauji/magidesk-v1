using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.Input;

namespace Magidesk.Presentation.ViewModels;

using Magidesk.Views;

public class OpenTicketsListViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetOpenTicketsQuery, IEnumerable<TicketDto>> _getOpenTicketsHandler;
    private readonly IQueryHandler<GetUsersQuery, IEnumerable<UserDto>> _getUsersHandler;
    private readonly ICommandHandler<TransferTicketCommand> _transferTicketHandler;
    private readonly NavigationService _navigationService;
    private readonly IUserService _userService;

    private ObservableCollection<TicketDto> _tickets = new();
    public ObservableCollection<TicketDto> Tickets
    {
        get => _tickets;
        set => SetProperty(ref _tickets, value);
    }

    private TicketDto? _selectedTicket;
    public TicketDto? SelectedTicket
    {
        get => _selectedTicket;
        set
        {
            if (SetProperty(ref _selectedTicket, value))
            {
                ((AsyncRelayCommand)ResumeCommand).NotifyCanExecuteChanged();
                ((AsyncRelayCommand)TransferCommand).NotifyCanExecuteChanged();
                ((AsyncRelayCommand)VoidCommand).NotifyCanExecuteChanged();
            }
        }
    }

    private ObservableCollection<UserDto> _users = new();
    public ObservableCollection<UserDto> Users
    {
        get => _users;
        set => SetProperty(ref _users, value);
    }

    private UserDto? _selectedUser;
    public UserDto? SelectedUser
    {
        get => _selectedUser;
        set => SetProperty(ref _selectedUser, value);
    }

    public ICommand ResumeCommand { get; }
    public ICommand TransferCommand { get; }
    public ICommand VoidCommand { get; }
    public ICommand SplitCommand { get; }
    public ICommand CloseCommand { get; }

    public OpenTicketsListViewModel(
        IQueryHandler<GetOpenTicketsQuery, IEnumerable<TicketDto>> getOpenTicketsHandler,
        IQueryHandler<GetUsersQuery, IEnumerable<UserDto>> getUsersHandler,
        ICommandHandler<TransferTicketCommand> transferTicketHandler,
        NavigationService navigationService,
        IUserService userService)
    {
        _getOpenTicketsHandler = getOpenTicketsHandler;
        _getUsersHandler = getUsersHandler;
        _transferTicketHandler = transferTicketHandler;
        _navigationService = navigationService;
        _userService = userService;

        ResumeCommand = new AsyncRelayCommand(ResumeAsync, () => SelectedTicket != null);
        TransferCommand = new AsyncRelayCommand(TransferAsync, () => SelectedTicket != null);
        VoidCommand = new AsyncRelayCommand(VoidAsync, () => SelectedTicket != null);
        SplitCommand = new AsyncRelayCommand(SplitAsync, () => SelectedTicket != null);
        CloseCommand = new RelayCommand(Close);

        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var tickets = await _getOpenTicketsHandler.HandleAsync(new GetOpenTicketsQuery());
        Tickets = new ObservableCollection<TicketDto>(tickets);

        var users = await _getUsersHandler.HandleAsync(new GetUsersQuery());
        Users = new ObservableCollection<UserDto>(users);
    }

    private async Task ResumeAsync()
    {
        if (SelectedTicket != null)
        {
            _navigationService.Navigate(typeof(Views.OrderEntryPage), SelectedTicket.Id);
            // Close dialog logic would be here if controlled by VM, but currently handled by View/Dialog result
        }
    }

    private async Task TransferAsync()
    {
        if (SelectedTicket == null) return;

        // Simple Transfer Dialog (or use a ContentDialog with ComboBox)
        // For MVP, we'll assume a ComboBox in a ContentDialog
        var userDialog = new ContentDialog
        {
            Title = "Transfer Ticket",
            PrimaryButtonText = "Transfer",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = App.MainWindowInstance.Content.XamlRoot
        };

        var stackPanel = new StackPanel { Spacing = 10 };
        var userCombo = new ComboBox 
        { 
            Header = "Select New Owner", 
            ItemsSource = Users, 
            DisplayMemberPath = "FullName", 
            HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch 
        };
        stackPanel.Children.Add(userCombo);
        userDialog.Content = stackPanel;

        var result = await _navigationService.ShowDialogAsync(userDialog);

        if (result == ContentDialogResult.Primary && userCombo.SelectedItem is UserDto selectedUser)
        {
            try
            {
                if (_userService.CurrentUser?.Id == null)
                {
                    return;
                }

                var command = new TransferTicketCommand
                {
                    TicketId = SelectedTicket.Id,
                    NewOwnerId = new UserId(selectedUser.Id),
                    TransferredBy = _userService.CurrentUser.Id
                };

                await _transferTicketHandler.HandleAsync(command);
                await LoadDataAsync(); // Refresh
            }
            catch (Exception ex)
            {
                // Show error
                System.Diagnostics.Debug.WriteLine($"Transfer Error: {ex.Message}");
            }
        }
    }

    private async Task VoidAsync()
    {
        if (SelectedTicket == null) return;

        var dialog = new Views.VoidTicketDialog();
        dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        dialog.ViewModel.Initialize(SelectedTicket);
        await _navigationService.ShowDialogAsync(dialog);
        
        // Refresh list
        await LoadDataAsync();
    }

    private async Task SplitAsync()
    {
        if (SelectedTicket == null) return;

        var dialog = new SplitTicketDialog();
        dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        dialog.ViewModel.Initialize(SelectedTicket);
        await _navigationService.ShowDialogAsync(dialog);

        // Refresh list
        await LoadDataAsync();
    }

    private void Close()
    {
        // Dialog close handled by View
    }
}
