using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// ViewModel for the Audit Log page.
/// Provides audit log viewing with filtering, search, and pagination.
/// </summary>
public partial class AuditLogViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetAuditLogsQuery, GetAuditLogsResult> _getAuditLogsHandler;
    private readonly IUserRepository _userRepository;

    // Filter properties
    private DateTimeOffset _startDate = DateTime.Today.AddDays(-7);
    private DateTimeOffset _endDate = DateTime.Today.AddDays(1).AddSeconds(-1);
    private Guid? _selectedUserId;
    private AuditEventType? _selectedEventType;
    private string? _selectedEntityType;
    private string _searchText = string.Empty;

    // Pagination properties
    private int _currentPage = 1;
    private int _pageSize = 50;
    private int _totalCount;
    private int _totalPages;

    // Data properties
    private ObservableCollection<AuditLogDto> _auditLogs = new();
    private AuditLogDto? _selectedAuditLog;
    private string _error = string.Empty;

    // User list for filter dropdown
    private ObservableCollection<UserFilterItem> _users = new();

    public AuditLogViewModel(
        IQueryHandler<GetAuditLogsQuery, GetAuditLogsResult> getAuditLogsHandler,
        IUserRepository userRepository)
    {
        _getAuditLogsHandler = getAuditLogsHandler ?? throw new ArgumentNullException(nameof(getAuditLogsHandler));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        Title = "Audit Log";

        // Initialize commands
        LoadAuditLogsCommand = new AsyncRelayCommand(LoadAuditLogsAsync);
        SearchCommand = new AsyncRelayCommand(SearchAsync);
        ClearFiltersCommand = new RelayCommand(ClearFilters);
        ExportCommand = new AsyncRelayCommand(ExportAsync);
        NextPageCommand = new AsyncRelayCommand(NextPageAsync, () => CurrentPage < TotalPages);
        PreviousPageCommand = new AsyncRelayCommand(PreviousPageAsync, () => CurrentPage > 1);
        FirstPageCommand = new AsyncRelayCommand(FirstPageAsync, () => CurrentPage > 1);
        LastPageCommand = new AsyncRelayCommand(LastPageAsync, () => CurrentPage < TotalPages);
    }

    public DateTimeOffset StartDate
    {
        get => _startDate;
        set => SetProperty(ref _startDate, value);
    }

    public DateTimeOffset EndDate
    {
        get => _endDate;
        set => SetProperty(ref _endDate, value);
    }

    public Guid? SelectedUserId
    {
        get => _selectedUserId;
        set => SetProperty(ref _selectedUserId, value);
    }

    public AuditEventType? SelectedEventType
    {
        get => _selectedEventType;
        set => SetProperty(ref _selectedEventType, value);
    }

    public string? SelectedEntityType
    {
        get => _selectedEntityType;
        set => SetProperty(ref _selectedEntityType, value);
    }

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            if (SetProperty(ref _currentPage, value))
            {
                NextPageCommand.NotifyCanExecuteChanged();
                PreviousPageCommand.NotifyCanExecuteChanged();
                FirstPageCommand.NotifyCanExecuteChanged();
                LastPageCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public int PageSize
    {
        get => _pageSize;
        set => SetProperty(ref _pageSize, value);
    }

    public int TotalCount
    {
        get => _totalCount;
        set => SetProperty(ref _totalCount, value);
    }

    public int TotalPages
    {
        get => _totalPages;
        set
        {
            if (SetProperty(ref _totalPages, value))
            {
                NextPageCommand.NotifyCanExecuteChanged();
                PreviousPageCommand.NotifyCanExecuteChanged();
                FirstPageCommand.NotifyCanExecuteChanged();
                LastPageCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public ObservableCollection<AuditLogDto> AuditLogs
    {
        get => _auditLogs;
        set => SetProperty(ref _auditLogs, value);
    }

    public AuditLogDto? SelectedAuditLog
    {
        get => _selectedAuditLog;
        set => SetProperty(ref _selectedAuditLog, value);
    }

    public string Error
    {
        get => _error;
        set
        {
            if (SetProperty(ref _error, value))
            {
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    public bool HasError => !string.IsNullOrEmpty(Error);

    public ObservableCollection<UserFilterItem> Users
    {
        get => _users;
        set => SetProperty(ref _users, value);
    }

    public ObservableCollection<AuditEventType> EventTypes { get; } = new(Enum.GetValues<AuditEventType>());

    public ObservableCollection<string> EntityTypes { get; } = new()
    {
        "Ticket",
        "Payment",
        "User",
        "Table",
        "MenuItem",
        "Order",
        "Session",
        "Discount",
        "Refund"
    };

    public AsyncRelayCommand LoadAuditLogsCommand { get; }
    public AsyncRelayCommand SearchCommand { get; }
    public RelayCommand ClearFiltersCommand { get; }
    public AsyncRelayCommand ExportCommand { get; }
    public AsyncRelayCommand NextPageCommand { get; }
    public AsyncRelayCommand PreviousPageCommand { get; }
    public AsyncRelayCommand FirstPageCommand { get; }
    public AsyncRelayCommand LastPageCommand { get; }

    /// <summary>
    /// Initializes the view model by loading users and initial audit logs.
    /// </summary>
    public async Task InitializeAsync()
    {
        await LoadUsersAsync();
        await LoadAuditLogsAsync();
    }

    /// <summary>
    /// Loads the list of users for the filter dropdown.
    /// </summary>
    private async Task LoadUsersAsync()
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            Users.Clear();
            Users.Add(new UserFilterItem { Id = null, Username = "All Users" });
            
            foreach (var user in users.OrderBy(u => u.Username))
            {
                Users.Add(new UserFilterItem { Id = user.Id, Username = user.Username });
            }
        }
        catch (Exception ex)
        {
            Error = $"Failed to load users: {ex.Message}";
        }
    }

    /// <summary>
    /// Loads audit logs based on current filters and pagination.
    /// </summary>
    private async Task LoadAuditLogsAsync()
    {
        IsBusy = true;
        Error = string.Empty;

        try
        {
            var query = new GetAuditLogsQuery(
                StartDate: StartDate.LocalDateTime,
                EndDate: EndDate.LocalDateTime,
                UserId: SelectedUserId,
                EventType: SelectedEventType,
                EntityType: SelectedEntityType,
                SearchText: string.IsNullOrWhiteSpace(SearchText) ? null : SearchText,
                PageNumber: CurrentPage,
                PageSize: PageSize
            );

            var result = await _getAuditLogsHandler.HandleAsync(query);

            AuditLogs.Clear();
            foreach (var log in result.AuditLogs)
            {
                AuditLogs.Add(log);
            }

            TotalCount = result.TotalCount;
            TotalPages = result.TotalPages;
        }
        catch (Exception ex)
        {
            Error = $"Failed to load audit logs: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Performs a search with current filters.
    /// </summary>
    private async Task SearchAsync()
    {
        CurrentPage = 1; // Reset to first page on new search
        await LoadAuditLogsAsync();
    }

    /// <summary>
    /// Clears all filters and reloads data.
    /// </summary>
    private void ClearFilters()
    {
        StartDate = DateTime.Today.AddDays(-7);
        EndDate = DateTime.Today.AddDays(1).AddSeconds(-1);
        SelectedUserId = null;
        SelectedEventType = null;
        SelectedEntityType = null;
        SearchText = string.Empty;
        CurrentPage = 1;
    }

    /// <summary>
    /// Exports audit logs to a file.
    /// </summary>
    private async Task ExportAsync()
    {
        IsBusy = true;
        Error = string.Empty;

        try
        {
            // Get all audit logs without pagination for export
            var query = new GetAuditLogsQuery(
                StartDate: StartDate.LocalDateTime,
                EndDate: EndDate.LocalDateTime,
                UserId: SelectedUserId,
                EventType: SelectedEventType,
                EntityType: SelectedEntityType,
                SearchText: string.IsNullOrWhiteSpace(SearchText) ? null : SearchText,
                PageNumber: 1,
                PageSize: int.MaxValue // Get all records
            );

            var result = await _getAuditLogsHandler.HandleAsync(query);

            // Create CSV content
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Timestamp,User,Event Type,Entity Type,Entity ID,Description,Correlation ID");

            foreach (var log in result.AuditLogs)
            {
                csv.AppendLine($"\"{log.Timestamp:yyyy-MM-dd HH:mm:ss}\",\"{log.UserName}\",\"{log.EventType}\",\"{log.EntityType}\",\"{log.EntityId}\",\"{EscapeCsv(log.Description)}\",\"{log.CorrelationId}\"");
            }

            // Save to file
            var fileName = $"AuditLog_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            var filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
            
            await System.IO.File.WriteAllTextAsync(filePath, csv.ToString());

            StatusMessage = $"Audit log exported to: {filePath}";
        }
        catch (Exception ex)
        {
            Error = $"Failed to export audit logs: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Navigates to the next page.
    /// </summary>
    private async Task NextPageAsync()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
            await LoadAuditLogsAsync();
        }
    }

    /// <summary>
    /// Navigates to the previous page.
    /// </summary>
    private async Task PreviousPageAsync()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            await LoadAuditLogsAsync();
        }
    }

    /// <summary>
    /// Navigates to the first page.
    /// </summary>
    private async Task FirstPageAsync()
    {
        if (CurrentPage > 1)
        {
            CurrentPage = 1;
            await LoadAuditLogsAsync();
        }
    }

    /// <summary>
    /// Navigates to the last page.
    /// </summary>
    private async Task LastPageAsync()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage = TotalPages;
            await LoadAuditLogsAsync();
        }
    }

    /// <summary>
    /// Escapes CSV special characters.
    /// </summary>
    private static string EscapeCsv(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return value.Replace("\"", "\"\"");
    }
}

/// <summary>
/// Helper class for user filter dropdown.
/// </summary>
public class UserFilterItem
{
    public Guid? Id { get; set; }
    public string Username { get; set; } = string.Empty;
}
