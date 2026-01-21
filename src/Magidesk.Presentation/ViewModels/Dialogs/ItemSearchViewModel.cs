using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.ViewModels;

namespace Magidesk.Presentation.ViewModels.Dialogs;

public class ItemSearchViewModel : ViewModelBase
{
    private readonly IMenuRepository _menuRepository;
    private string _searchText = string.Empty;
    private ObservableCollection<MenuItem> _results = new();
    private MenuItem? _selectedItem;

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                _ = PerformSearchAsync();
            }
        }
    }

    public ObservableCollection<MenuItem> Results
    {
        get => _results;
        private set => SetProperty(ref _results, value);
    }

    public MenuItem? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value) && value != null)
            {
                // Auto-close on selection logic handled by View or Command
                ConfirmSelectionCommand.Execute(null);
            }
        }
    }

    public ICommand ConfirmSelectionCommand { get; }
    public ICommand CancelCommand { get; }
    
    // Close Action
    public System.Action? CloseAction { get; set; }

    public ItemSearchViewModel(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
        Title = "Item Search";
        
        ConfirmSelectionCommand = new RelayCommand(() => CloseAction?.Invoke());
        CancelCommand = new RelayCommand(() => { SelectedItem = null; CloseAction?.Invoke(); });
        
        // Initial load? No, wait for input or load all if fast?
        // Audit says "User types search query".
    }

    private async Task PerformSearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            Results.Clear();
            return;
        }

        IsBusy = true;
        try
        {
            // Naive in-memory filter of ALL items for responsiveness if dataset small
            // Or repository query. Given current repository interface, let's use GetAll and filter.
            // Optimization: Cache all items if possible? 
            // For now, fetch all then filter. 
            var allItems = await _menuRepository.GetAllAsync();
            
            var query = SearchText.Trim();
            var matches = allItems.Where(i => 
                (i.Name?.Contains(query, System.StringComparison.OrdinalIgnoreCase) ?? false) ||
                (i.Barcode?.Contains(query, System.StringComparison.OrdinalIgnoreCase) ?? false)
            ).ToList();

            Results = new ObservableCollection<MenuItem>(matches);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
